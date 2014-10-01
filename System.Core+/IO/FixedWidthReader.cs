using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
	/// <summary>
	/// Reads flat files with fixed width columns
	/// </summary>
	public class FixedWidthReader : FlatFileReader
	{
		private IReader reader;

		private int[] columnWidths;
		private int recordWidth;
		private char[] padding;

        public FixedWidthReader(string filePath, params int[] columnWidths)
            : this(File.OpenRead(filePath), columnWidths) { }

		public FixedWidthReader(Stream s, params int[] columnWidths)
            : this(s, new FixedWidthReaderOptions(), columnWidths) { }

		public FixedWidthReader(Stream s, FixedWidthReaderOptions options, params int[] columnWidths)
			: base(s)
        {
			if (columnWidths == null || columnWidths.Length == 0)
				throw new ArgumentNullException("columnWidths");

			if (columnWidths.Any(i => i < 1))
				throw new ArgumentOutOfRangeException("columnWidths", "Column widths must be greater than zero");

			this.reader = new StreamReaderAdapter(new StreamReader(s, options.Encoding));
			this.columnWidths = columnWidths;
			this.recordWidth = columnWidths.Sum();
			this.padding = options.Padding;
        }

		public override string[] Read()
		{
			CheckDisposed();

			if (reader.EndOfStream)
				return null;

			var rec = reader.Read(recordWidth);

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

		protected override void OnDispose()
		{
			if (this.reader != null)
				this.reader.Dispose();
		}
	}
}
