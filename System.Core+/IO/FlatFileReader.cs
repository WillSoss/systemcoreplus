using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
	public abstract class FlatFileReader : IDisposable
	{
		private volatile bool disposed = false;

		public Stream BaseStream { get; private set; }

		public FlatFileReader(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			this.BaseStream = stream;
		}

		public abstract string[] Read();

		/// <summary>
		/// Consumes the end-of-line characters from the reader. Will consume CR+LF or just CR or LF, whichever is present.
		/// </summary>
		internal void ConsumeEol(IReader Reader)
		{
			if (!Reader.EndOfStream)
			{
				var next = (char)Reader.Peek();

				if (next == '\r')
				{
					Reader.Read();

					if (!Reader.EndOfStream)
						next = (char)Reader.Peek();
				}

				if (next == '\n')
					Reader.Read();
			}
		}

		protected void CheckDisposed()
		{
			if (this.disposed)
				throw new InvalidOperationException("Cannot call property or method after the object is disposed");
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;

				OnDispose();
			}
		}

		protected virtual void OnDispose() { }
	}
}
