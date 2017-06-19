using System;
using System.Collections.Generic;
using System.IO.Parsing;
using System.Linq;
using System.Text;

namespace System.IO
{
	public class FileFormatWriter : IDisposable
	{
		public IFlatFileWriter writer;
		private Dictionary<Type, FieldMappingList> recordFieldMappings;

		public bool IsDisposed { get; private set; }

		public bool MultipleRecordTypes { get { return recordFieldMappings.Count > 1; } }

		public int RecordCount { get; private set; }

		public FileFormatWriter(string filePath, FlatFileFormat format, params Type[] recordTypes)
		{
			IsDisposed = false;
			recordFieldMappings = new Dictionary<Type, FieldMappingList>();
			Dictionary<string, int[]> multiRecordColumnWidths = new Dictionary<string, int[]>();

			// TODO: Add exception when key field doesnt return a value (multiple nulls or default values)
			foreach (var type in recordTypes)
			{
				var mapping = new FieldMappingList(type);
				recordFieldMappings.Add(mapping.RecordType, mapping);
				multiRecordColumnWidths.Add(mapping.Key, mapping.FieldLengths.ToArray());
			}

			if (format == FlatFileFormat.Csv)
			{
				// TODO: Support for CSV in FileFormatWriter
				throw new NotImplementedException();
			}
			else if (format == FlatFileFormat.FixedWidth)
			{
				if (MultipleRecordTypes)
					writer = new FixedWidthWriter(filePath, multiRecordColumnWidths);
				else
					writer = new FixedWidthWriter(filePath, multiRecordColumnWidths.First().Value);
			}
			else
			{
				throw new ArgumentException("Unsupported file format");
			}
		}

		public void Write<T>(T record)
		{
			CheckDisposed();

			if (!recordFieldMappings.ContainsKey(typeof(T)))
				throw new ArgumentException($"Unknown type: {typeof(T).FullName}", nameof(record));

			var mappings = recordFieldMappings[typeof(T)];

			WriteValues(record, mappings);

			RecordCount++;
		}

		private void WriteValues(object record, FieldMappingList mappings)
		{
			throw new NotImplementedException();
		}

		public void Flush()
		{
			CheckDisposed();

			writer.Flush();
		}

		private void CheckDisposed()
		{
			if (IsDisposed)
				throw new InvalidOperationException("Cannot call property or method after the object is disposed");
		}

		public void Dispose()
		{
			lock (this)
			{
				if (!IsDisposed)
				{
					IsDisposed = true;

					if (writer != null)
					{
						writer.Dispose();
						writer = null;
					}
				}
			}
		}
	}
}
