using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using pelazem.util;
using pelazem.syngen.interfaces;
using pelazem.telemetry;
using pelazem.telemetry.log4netSink;

namespace pelazem.syngen.sink.file
{
	public class SinkFile : ISink
	{
		private Log4NetTelemetrySink _telemetry = null;

		internal ITelemetrySink Telemetry
		{
			get
			{
				if (_telemetry == null)
					_telemetry = new Log4NetTelemetrySink();

				return _telemetry;
			}
		}

		#region ISink

		private bool Validate(Stream content, ISinkConfig config)
		{
			bool result = false;
			string msg = string.Empty;

			if (content == null)
				msg = Properties.Resources.Error_ParamWasNull + nameof(content);
			else if (!content.CanRead)
				msg = Properties.Resources.Error_ParamWasUnreadable + nameof(content);
			else if (config == null)
				msg = Properties.Resources.Error_ParamWasNull + nameof(config);
			else if ((config as SinkFileConfig) == null)
				msg = Properties.Resources.Error_ParamTypeWrong + nameof(config)  + " -> " + nameof(SinkFileConfig);
			else
				result = true;

			// Only send telemetry if fail
			if (!result)
				this.Telemetry.Send(new TelemetryEvent() { EventTypeValue = TelemetryEvent.EventTypes.Event, LevelValue = TelemetryEvent.Levels.Error, Message = msg, Success = false });

			return result;
		}

		private bool HandleSinkFolder(SinkFileConfig sinkFileConfig)
		{
			bool result = false;

			string fullFolderPath = Path.GetDirectoryName(sinkFileConfig.FileUri);

			try
			{
				if (!Directory.Exists(fullFolderPath))
					Directory.CreateDirectory(fullFolderPath);

				result = Directory.Exists(fullFolderPath);
			}
			catch (Exception ex)
			{
				result = false;

				this.Telemetry.Send(new TelemetryEvent() { EventTypeValue = TelemetryEvent.EventTypes.Exception, Exception = ex, Message = Properties.Resources.Error_DirectoryCreate, Success = false });
			}

			return result;
		}

		public async Task<bool> SendAsync(Stream content, ISinkConfig config)
		{
			// Validate stream and config
			bool validationResult = Validate(content, config);

			if (!validationResult)
				return false;


			SinkFileConfig sinkFileConfig = config as SinkFileConfig;


			// Ensure sink file folder exists - return if cannot
			bool sinkFileResult = HandleSinkFolder(sinkFileConfig);

			if (!(sinkFileResult))
				return false;


			bool result = false;

			var fi = new FileInfo(sinkFileConfig.FileUri);


			// If sink file already exists, handle according to behavior specified on sink config
			if (fi.Exists)
			{
				if (sinkFileConfig.BehaviorIfFileExists == SinkFileConfig.IfFileExists.DoNothing)
				{
					this.Telemetry.Send(new TelemetryEvent() { EventTypeValue = TelemetryEvent.EventTypes.Event, LevelValue = TelemetryEvent.Levels.Warning, Message = Properties.Resources.Warn_FileAlreadyExistedSoDataNotSentToSink, Success = true });

					return false;
				}
				else if (sinkFileConfig.BehaviorIfFileExists == SinkFileConfig.IfFileExists.Error)
				{
					this.Telemetry.Send(new TelemetryEvent() { EventTypeValue = TelemetryEvent.EventTypes.Event, LevelValue = TelemetryEvent.Levels.Error, Message = Properties.Resources.Error_FileAlreadyExistedSoDataNotSentToSink, Success = false });

					return false;
				}
				else if (sinkFileConfig.BehaviorIfFileExists == SinkFileConfig.IfFileExists.Replace)
				{
					fi.Delete();

					this.Telemetry.Send(new TelemetryEvent() { EventTypeValue = TelemetryEvent.EventTypes.Event, LevelValue = TelemetryEvent.Levels.Warning, Message = Properties.Resources.Warn_FileAlreadyExistedAndWasDeleted, Success = true });
				}
			}


			// Write the data to the sink
			using (FileStream fs = new FileStream(sinkFileConfig.FileUri, FileMode.Append, FileAccess.Write, FileShare.Read))
			{
				byte[] bytes;

				using (var memoryStream = new MemoryStream())
				{
					await content.CopyToAsync(memoryStream);

					bytes = memoryStream.ToArray();
				}

				try
				{
					await fs.WriteAsync(bytes, 0, bytes.Length);

					result = true;
				}
				catch (Exception ex)
				{
					result = false;

					this.Telemetry.Send(new TelemetryEvent() { EventTypeValue = TelemetryEvent.EventTypes.Exception, Exception = ex, Message = Properties.Resources.Error_FailureWritingDataToSink, Success = false });
				}
			}

			return result;
		}

		#endregion

	}
}
