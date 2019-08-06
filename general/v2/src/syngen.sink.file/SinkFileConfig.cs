using pelazem.syngen.interfaces;
using System;

namespace pelazem.syngen.sink.file
{
	public class SinkFileConfig : ISinkConfig
	{
		[Flags]
		public enum IfFileExists
		{
			AppendOrReplace,
			Replace,
			DoNothing,
			Error
		}

		public string FileUri { get; set; }

		public IfFileExists BehaviorIfFileExists { get; set; } = IfFileExists.DoNothing; 
	}
}
