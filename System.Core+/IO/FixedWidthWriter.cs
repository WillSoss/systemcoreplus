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
		private bool firstRecord = true;
		private volatile bool disposed = false;

		private readonly int[] columnWidths;
		private readonly Dictionary<string, int[]> multiRecordColumnWidths;
		private char padchar;
		private string newLine;
		private bool truncate;
		private ValueAlignment alignment;

		/// <summary>
		/// Initializes a new FixedWidthWriter with a single set of column widths for all records in the file.
		/// </summary>
		public FixedWidthWriter(string filePath, params int[] columnWidths)
			: this(filePath, FileMode.CreateNew, columnWidths) { }

		/// <summary>
		/// Initializes a new FixedWidthWriter with a single set of column widths for all records in the file.
		/// </summary>
		public FixedWidthWriter(string filePath, FileMode mode, params int[] columnWidths)
			: this(File.Open(filePath, mode), columnWidths) { }

		/// <summary>
		/// Initializes a new FixedWidthWriter with a single set of column widths for all records in the file.
		/// </summary>
		public FixedWidthWriter(Stream s, params int[] columnWidths)
			: this(s, new FixedWidthWriterOptions(), columnWidths) { }

		/// <summary>
		/// Initializes a new FixedWidthWriter with a single set of column widths for all records in the file.
		/// </summary>
		public FixedWidthWriter(Stream s, FixedWidthWriterOptions options, params int[] columnWidths)
			: this(s, options)
		{
			if (columnWidths == null || columnWidths.Length == 0)
				throw new ArgumentNullException("columnWidths");

			if (columnWidths.Any(i => i < 1))
				throw new ArgumentOutOfRangeException("columnWidths", "Column widths must be greater than zero");
			
			this.columnWidths = columnWidths;
        }

		/// <summary>
		/// Initializes a new FixedWidthWriter with multiple types of records, each defining their own column widths. The key in the dictionary must correspond to the value of the first column of the record.
		/// </summary>
		public FixedWidthWriter(string filePath, Dictionary<string, int[]> multiRecordColumnWidths)
			: this(filePath, FileMode.CreateNew, multiRecordColumnWidths) { }

		/// <summary>
		/// Initializes a new FixedWidthWriter with multiple types of records, each defining their own column widths. The key in the dictionary must correspond to the value of the first column of the record.
		/// </summary>
		public FixedWidthWriter(string filePath, FileMode mode, Dictionary<string, int[]> multiRecordColumnWidths)
			: this(File.Open(filePath, mode), multiRecordColumnWidths) { }

		/// <summary>
		/// Initializes a new FixedWidthWriter with multiple types of records, each defining their own column widths. The key in the dictionary must correspond to the value of the first column of the record.
		/// </summary>
		public FixedWidthWriter(Stream s, Dictionary<string, int[]> multiRecordColumnWidths)
			: this(s, new FixedWidthWriterOptions(), multiRecordColumnWidths) { }

		/// <summary>
		/// Initializes a new FixedWidthWriter with multiple types of records, each defining their own column widths. The key in the dictionary must correspond to the value of the first column of the record.
		/// </summary>
		public FixedWidthWriter(Stream s, FixedWidthWriterOptions options, Dictionary<string, int[]> multiRecordColumnWidths)
			: this(s, options)
		{
			if (multiRecordColumnWidths == null || multiRecordColumnWidths.Count == 0)
				throw new ArgumentNullException(nameof(multiRecordColumnWidths));

			if (multiRecordColumnWidths.Select(kv => kv.Value).Any(v => v.Length == 0))
				throw new ArgumentOutOfRangeException(nameof(multiRecordColumnWidths), "All column width arrays must contain at least one value");

			if (multiRecordColumnWidths.SelectMany(kv => kv.Value).Any(w => w < 1))
				throw new ArgumentOutOfRangeException(nameof(multiRecordColumnWidths), "Column widths must be greater than zero");

			if (multiRecordColumnWidths.Any(kv => kv.Key.Length > kv.Value[0]))
				throw new ArgumentOutOfRangeException(nameof(multiRecordColumnWidths), "Record identifiers (keys) cannot be longer than the corresponding first column width");
			
			this.multiRecordColumnWidths = multiRecordColumnWidths;
		}

		private FixedWidthWriter(Stream s, FixedWidthWriterOptions options)
		{
			this.writer = new StreamWriter(s, options.Encoding);
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

			int[] columnWidths = this.columnWidths;
			
			StringBuilder flatrec = new StringBuilder();
			int i = 0;

			foreach (var obj in record)
			{
				// Do initialization here in order to access first item in record
				if (i == 0 && columnWidths == null)
				{
					columnWidths = multiRecordColumnWidths[obj.ToString()];

					if (columnWidths == null)
						throw new ArgumentException($"Could not find column widths for record type '{obj.ToString()}'", nameof(record));
				}

				if (i == columnWidths.Length)
					throw new ArgumentOutOfRangeException(nameof(record), "Contains too many fields");

				string value = (obj ?? string.Empty).ToString();

				if (value.Length > columnWidths[i])
				{
					if (truncate)
						value = value.Truncate(columnWidths[i]);
					else
						throw new ArgumentOutOfRangeException(nameof(record), $"Value in field at index {i} is too long. Field length is {columnWidths[i]}, value length is {value.Length}, value is '{value}'.");
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
				throw new ArgumentOutOfRangeException(nameof(record), "Contains too few fields");

			// Put a newline between records, but not before or after a record in a file with one record
			if (firstRecord)
				firstRecord = false;
			else
				writer.WriteLine(newLine);

			writer.Write(flatrec.ToString());
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
