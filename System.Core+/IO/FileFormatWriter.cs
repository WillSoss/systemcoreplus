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
			: this(File.Open(filePath, FileMode.Create), format, recordTypes) { }

		public FileFormatWriter(Stream stream, FlatFileFormat format, params Type[] recordTypes)
		{
			IsDisposed = false;
			recordFieldMappings = new Dictionary<Type, FieldMappingList>();
			Dictionary<string, int[]> multiRecordColumnWidths = new Dictionary<string, int[]>();

			// TODO: Add exception when key field doesnt return a value (multiple nulls or default values)
			foreach (var type in recordTypes)
			{
				var mapping = new FieldMappingList(type);
				recordFieldMappings.Add(mapping.RecordType, mapping);

				if (mapping.Key == null && recordTypes.Length > 1)
					throw new FieldMappingException($"{type.FullName} does not supply a value in field 0 for mapping record types");

				multiRecordColumnWidths.Add(mapping.Key ?? string.Empty, mapping.FieldLengths.ToArray());
			}

			if (format == FlatFileFormat.Csv)
			{
				// TODO: Support for CSV in FileFormatWriter
				throw new NotImplementedException();
			}
			else if (format == FlatFileFormat.FixedWidth)
			{
				if (MultipleRecordTypes)
					writer = new FixedWidthWriter(stream, multiRecordColumnWidths);
				else
					writer = new FixedWidthWriter(stream, multiRecordColumnWidths.First().Value);
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

			var output = WriteValues(record, mappings);

			writer.Write(output);

			RecordCount++;
		}

		private object[] WriteValues(object item, FieldMappingList mappings, object[] output = null, int offset = 0)
		{
			if (output == null)
				output = new object[mappings.FieldCount];

			foreach (var field in mappings)
			{
				if (field.IsArray)
				{
					Array array = field.GetArray(item);

					for (int i = 0; i < field.ElementCount; i++)
					{
						if (field.IsComplexType)
						{
							WriteValues(array.GetValue(i), field.InnerMappings, output, i * field.FieldCount);
						}
						else
						{
							output[field.AbsoluteIndex + offset + i] = array.GetValue(i);
						}
					}
				}
				else if (field.IsComplexType)
				{
					WriteValues(field.GetValue(item), field.InnerMappings, output, 0);
				}
				else
				{
					output[field.AbsoluteIndex + offset] = field.GetValue(item);
				}
			}

			return output;
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
