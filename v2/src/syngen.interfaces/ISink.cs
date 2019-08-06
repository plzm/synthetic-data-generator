using System.IO;
using System.Threading.Tasks;

namespace pelazem.syngen.interfaces
{
	public interface ISink
	{
		Task<bool> SendAsync(Stream content, ISinkConfig sinkConfig);
	}
}
