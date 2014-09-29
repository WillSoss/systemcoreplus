using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.IO
{
	internal class BinaryReaderAdapter : IReader
	{
		private readonly BinaryReader reader;
		private bool endOfStream = false;

		internal BinaryReaderAdapter(BinaryReader reader)
		{
			this.reader = reader;
		}

		Stream IReader.BaseStream
		{
			get { return reader.BaseStream; }
		}

		int IReader.Read()
		{
			int i = reader.Read();

			if (i == -1)
				endOfStream = true;

			return i;
		}

		int IReader.Peek()
		{
			return reader.PeekChar();
		}

		bool IReader.EndOfStream
		{
			get { return endOfStream; }
		}

		void IDisposable.Dispose()
		{
			reader.Dispose();
		}
	}
}
