using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.IO
{
	internal class StreamReaderAdapter : IReader
	{
		private readonly StreamReader reader;

		internal StreamReaderAdapter(StreamReader reader)
		{
			this.reader = reader;
		}

		Stream IReader.BaseStream
		{
			get { return reader.BaseStream; }
		}

		int IReader.Read()
		{
			return reader.Read();
		}

		string IReader.Read(int length)
		{
			char[] buffer = new char[length];
			var read = reader.ReadBlock(buffer, 0, length);
			return new string(buffer, 0, Math.Min(length, read));
		}

		int IReader.Peek()
		{
			return reader.Peek();
		}

		bool IReader.EndOfStream
		{
			get { return reader.EndOfStream; }
		}

		void IDisposable.Dispose()
		{
			reader.Dispose();
		}
	}
}
