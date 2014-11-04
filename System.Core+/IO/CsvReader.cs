using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.IO
{
	/// <summary>
	/// Reads flat files with comma separated values
	/// </summary>
	public class CsvReader : IFlatFileReader
	{
		public const char DefaultQualifier = '"';
		public const char DefaultDelimiter = ',';

		private volatile bool disposed = false;
		private IReader reader;

		private char qualifier;
		private char delimiter;

        public CsvReader(string filePath)
            : this(File.OpenRead(filePath)) { }

		public CsvReader(Stream s)
			: this(s, DefaultQualifier, DefaultDelimiter) { }

        public CsvReader(Stream s, char qualifier, char delimiter)
            : this(s, qualifier, delimiter, Encoding.UTF8) { }

        public CsvReader(Stream s, char qualifier, char delimiter, Encoding encoding)
        {
            this.reader = new StreamReaderAdapter(new StreamReader(s, encoding));
            this.delimiter = delimiter;
            this.qualifier = qualifier;
        }

		public Stream BaseStream { get { return reader.BaseStream; } }

		public bool EndOfFile { get { return reader.EndOfStream; } }
		
		public string[] Read()
		{
			CheckDisposed();

			if (reader.EndOfStream)
				return null;

			List<string> record = new List<string>();
			StringBuilder field = new StringBuilder();

			bool qualified = false;
			bool escapedQualifier = false;
			bool eatUntilDelimiter = false;

			while (!reader.EndOfStream)
			{
				char c = (char)reader.Read();

				if (!qualified && c == delimiter)
				{
					record.Add(field.ToString());
					eatUntilDelimiter = false;
					field.Clear();
					continue;
				}

				if (eatUntilDelimiter)
					continue;

				if (c == qualifier)
				{
					if (escapedQualifier)
					{
						escapedQualifier = false;
						continue;
					}
					else if (!qualified)
					{
						qualified = true;
						field.Clear(); // dump anything before qualifier after delimeter
						continue;
					}
					else if (reader.Peek() == -1 || ((char)reader.Peek()) != qualifier)
					{
						qualified = false;
						eatUntilDelimiter = true;
						continue;
					}
					else
					{
						escapedQualifier = true;
					}
				}

				if (!qualified)
				{
					if (c == '\n' || c == '\r')
					{
						if (reader.Peek() != -1)
						{
							char next = (char)reader.Peek();

							if ((c == '\r' && next == '\n') || (c == '\n' && next == '\r'))
								reader.Read();
						}

						break;
					}
				}

				field.Append(c);
			}

			record.Add(field.ToString());

			return record.ToArray();
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

				if (this.reader != null)
					this.reader.Dispose();
			}
		}
	}
}
