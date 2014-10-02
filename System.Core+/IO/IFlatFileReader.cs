
namespace System.IO
{
	public interface IFlatFileReader : IDisposable
	{
		bool EndOfFile { get; }
		Stream BaseStream { get; }
		string[] Read();
	}
}
