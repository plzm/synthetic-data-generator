using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using pelazem.syngen.sink.file;

namespace syngen.sink.file.tester
{
	class Program
	{
		static void Main(string[] args)
		{
			DoIt().Wait();

			Console.WriteLine("Done. Press any key to exit.");
			Console.ReadKey();
		}

		private static async Task DoIt()
		{
			SinkFileConfig config = new SinkFileConfig() { BehaviorIfFileExists = SinkFileConfig.IfFileExists.AppendOrReplace, FileUri = @"d:\test\output.txt" };
			SinkFile sink = new SinkFile();

			for (int i = 1; i <= 10000; i++)
			{
				using (Stream s = GetStream("Content #" + i.ToString() + Environment.NewLine))
				{
					await sink.SendAsync(s, config);
				}
			}
		}

		private static MemoryStream GetStream(string content)
		{
			MemoryStream result = new MemoryStream();
			result.Write(Encoding.UTF8.GetBytes(content));
			result.Seek(0, SeekOrigin.Begin);
			return result;
		}
	}
}
