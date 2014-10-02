using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
	/// <summary>
	/// Writes flat files with fixed width columns
	/// </summary>
	public class FixedWidthWriter : IFlatFileWriter
	{
		public const char DefaultPadding = ' ';
		public static readonly string DefaultNewLine = Environment.NewLine;
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;

		private StreamWriter writer;
		private volatile bool disposed = false;

		private int[] columnWidths;
		private char padchar;
		private string newLine;
		private bool truncate;
		private ValueAlignment alignment;

		public FixedWidthWriter(string filePath, params int[] columnWidths)
			: this(filePath, FileMode.CreateNew, columnWidths) { }

		public FixedWidthWriter(string filePath, FileMode mode, params int[] columnWidths)
			: this(File.Open(filePath, mode), columnWidths) { }

		public FixedWidthWriter(Stream s, params int[] columnWidths)
			: this(s, new FixedWidthWriterOptions(), columnWidths) { }

		public FixedWidthWriter(Stream s, FixedWidthWriterOptions options, params int[] columnWidths)
		{
			if (columnWidths == null || columnWidths.Length == 0)
				throw new ArgumentNullException("columnWidths");

			if (columnWidths.Any(i => i < 1))
				throw new ArgumentOutOfRangeException("columnWidths", "Column widths must be greater than zero");

            this.writer = new StreamWriter(s, options.Encoding);
			this.columnWidths = columnWidths;
			this.padchar = options.Padding;
            this.newLine = options.NewLine;
			this.truncate = options.TruncateLongValues;
			this.alignment = options.ValueAlignment;
        }

		public Stream BaseStream { get { return writer.BaseStream; } }

		/// <summary>
		/// Writes the objects out as a single record in the file
		/// </summary>
		/// <param name="record">A single record where each item is a single field</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the data contains too few or too many fields, or a field is too long for the column and TruncateLongValues is not set to true.</exception>
        public void Write(params string[] record)
        {
            Write((IEnumerable)record);
        }

		/// <summary>
		/// Writes the objects out as a single record
		/// </summary>
		/// <param name="record">A single record where each item is a single field</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the data contains too few or too many fields, or a field is too long for the column and TruncateLongValues is not set to true.</exception>
        public void Write(params object[] record)
        {
            Write((IEnumerable)record);
        }

		/// <summary>
		/// Writes the objects out as a single record
		/// </summary>
		/// <param name="record">A single record where each item is a single field</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the data contains too few or too many fields, or a field is too long for the column and TruncateLongValues is not set to true.</exception>
		public void Write(IEnumerable record)
		{
			CheckDisposed();

			string[] values = new string[columnWidths.Length];
			StringBuilder flatrec = new StringBuilder();
			int i = 0;

			foreach (var obj in record)
			{
				if (i == columnWidths.Length)
					throw new ArgumentOutOfRangeException("record", "Contains too many fields");

				string value = (obj ?? string.Empty).ToString();

				if (value.Length > columnWidths[i])
				{
					if (truncate)
						value = value.Truncate(columnWidths[i]);
					else
						throw new ArgumentOutOfRangeException("record", "Field at index {0} is too long".FormatString(columnWidths[i]));
				}
				else if (value.Length < columnWidths[i])
				{
					var padding = new string(padchar, columnWidths[i] - value.Length);
					value = (alignment == ValueAlignment.Right ? padding : string.Empty) + value + (alignment == ValueAlignment.Left ? padding : string.Empty);
				}

				flatrec.Append(value);

				i++;
			}

			if (i != columnWidths.Length)
				throw new ArgumentOutOfRangeException("record", "Contains too few fields");
			
			writer.Write(flatrec.ToString());
			writer.Write(newLine);
		}

		/// <summary>
		/// Causes any buffered data to be written to the underlying stream.
		/// </summary>
		public void Flush()
		{
			writer.Flush();
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

				if (this.writer != null)
					this.writer.Dispose();
			}
		}
	}
}
