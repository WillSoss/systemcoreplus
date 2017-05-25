using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
	/// <summary>
	/// Reads flat files with fixed width columns
	/// </summary>
	public class FixedWidthReader : IFlatFileReader
	{
		private volatile bool disposed = false;
		private IReader reader;

		private readonly int[] columnWidths;
		private int recordWidth;
		private readonly Dictionary<string, int[]> multiRecordColumnWidths;
		private int firstColWidth;
		private char[] padding;

        public FixedWidthReader(string filePath, params int[] columnWidths)
            : this(File.OpenRead(filePath), columnWidths) { }

		public FixedWidthReader(Stream s, params int[] columnWidths)
            : this(s, new FixedWidthReaderOptions(), columnWidths) { }

		public FixedWidthReader(Stream s, FixedWidthReaderOptions options, params int[] columnWidths)
			: this(s, options)
        {
			if (columnWidths == null || columnWidths.Length == 0)
				throw new ArgumentNullException("columnWidths");

			if (columnWidths.Any(i => i < 1))
				throw new ArgumentOutOfRangeException("columnWidths", "Column widths must be greater than zero");
			
			this.columnWidths = columnWidths;
			this.recordWidth = columnWidths.Sum();
			this.firstColWidth = 0;
        }

		public FixedWidthReader(string filePath, Dictionary<string, int[]> multiRecordColumnWidths)
			: this(File.OpenRead(filePath), multiRecordColumnWidths) { }

		public FixedWidthReader(Stream s, Dictionary<string, int[]> multiRecordColumnWidths)
			: this(s, new FixedWidthReaderOptions(), multiRecordColumnWidths) { }

		public FixedWidthReader(Stream s, FixedWidthReaderOptions options, Dictionary<string, int[]> multiRecordColumnWidths)
		{
			if (multiRecordColumnWidths == null || multiRecordColumnWidths.Count == 0)
				throw new ArgumentNullException(nameof(multiRecordColumnWidths));

			if (multiRecordColumnWidths.Select(kv => kv.Value).Any(v => v.Length == 0))
				throw new ArgumentOutOfRangeException(nameof(multiRecordColumnWidths), "All column width arrays must contain at least one value");

			if (multiRecordColumnWidths.SelectMany(kv => kv.Value).Any(w => w < 1))
				throw new ArgumentOutOfRangeException(nameof(multiRecordColumnWidths), "Column widths must be greater than zero");

			if (multiRecordColumnWidths.Any(kv => kv.Key.Length > kv.Value[0]))
				throw new ArgumentOutOfRangeException(nameof(multiRecordColumnWidths), "Record identifiers (keys) cannot be longer than the corresponding first column width");

			this.firstColWidth = multiRecordColumnWidths.First().Value[0];

			if (multiRecordColumnWidths.Any(kv => kv.Value[0] != firstColWidth))
				throw new ArgumentOutOfRangeException(nameof(multiRecordColumnWidths), "The first column (key field) of each record must be the same size");

			this.multiRecordColumnWidths = multiRecordColumnWidths;
		}

		private FixedWidthReader(Stream s, FixedWidthReaderOptions options)
		{
			this.reader = new StreamReaderAdapter(new StreamReader(s, options.Encoding));
			this.padding = options.Padding;
		}

		public Stream BaseStream { get { return reader.BaseStream; } }

		public bool EndOfFile { get { return reader.EndOfStream; } }

		public string[] Read()
		{
			CheckDisposed();

			if (reader.EndOfStream)
				return null;

			int[] columnWidths = this.columnWidths;
			int recordWidth = this.recordWidth;
			string rec = string.Empty;

			if (columnWidths == null)
			{
				rec = reader.Read(firstColWidth);

				if (!multiRecordColumnWidths.ContainsKey(rec))
					throw new InvalidFileFormatException($"Unknown record type in file: {rec}");

				columnWidths = multiRecordColumnWidths[rec];
				recordWidth = columnWidths.Sum();
			}

			rec += reader.Read(recordWidth - firstColWidth);

			if (rec.Length < recordWidth)
				throw new InvalidFileFormatException("Unexpected end of record. Expected record length is {0}, record is {1} characters: '{2}'".FormatString(recordWidth, rec.Length, rec));

			var record = new string[columnWidths.Length];
			int pos = 0;

			for (int i = 0; i < columnWidths.Length; i++)
			{
				record[i] = rec.Substring(pos, columnWidths[i]).Trim(padding);
				pos += columnWidths[i];
			}

			ConsumeEol(reader);

			return record;
		}

		/// <summary>
		/// Consumes the end-of-line characters from the reader. Will consume CR+LF or just CR or LF, whichever is present.
		/// </summary>
		private void ConsumeEol(IReader Reader)
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

				if (this.reader != null)
					this.reader.Dispose();
			}
		}
	}
}
