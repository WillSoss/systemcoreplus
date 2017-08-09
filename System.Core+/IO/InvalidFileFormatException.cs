namespace System.IO
{
	public class InvalidFileFormatException : IOException
	{
		public InvalidFileFormatException(string message)
			: base(message) { }
	}
}
