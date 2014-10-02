using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO.Parsing
{
	public class FileParsingException : IOException
	{
		public FileParsingException(string message)
			: base(message) { }
	}
}
