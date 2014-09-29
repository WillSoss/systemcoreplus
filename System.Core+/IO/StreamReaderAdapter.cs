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
