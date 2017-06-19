using System.Collections;
using System.Collections.Generic;
using System.IO.Parsing;
using System.Linq;

namespace System.IO
{
	public class FileFormatReader : IDisposable, IEnumerator
	{
		private IFlatFileReader reader;
		private Dictionary<string, FieldMappingList> recordFieldMappings;
		private object current;

		public bool IsDisposed { get; private set; }

		public object Current
		{
			get
			{
				CheckDisposed();

				return current;
			}
		}

		public bool MultipleRecordTypes { get { return recordFieldMappings.Count > 1; } }

		public FileFormatReader(string filePath, FlatFileFormat format, params Type[] recordTypes)
		{
			IsDisposed = false;
			recordFieldMappings = new Dictionary<string, FieldMappingList>();
			Dictionary<string, int[]> multiRecordColumnWidths = new Dictionary<string, int[]>();

			// TODO: Add exception when key field doesnt return a value (multiple nulls or default values)
			foreach (var type in recordTypes)
			{
				var mapping = new FieldMappingList(type);
				recordFieldMappings.Add(mapping.Key, mapping);
				multiRecordColumnWidths.Add(mapping.Key, mapping.FieldLengths.ToArray());
			}

			if (format == FlatFileFormat.Csv)
			{
				// TODO: Support for CSV in FileFormatReader
				throw new NotImplementedException();
			}
			else if (format == FlatFileFormat.FixedWidth)
			{
				if (MultipleRecordTypes)
					reader = new FixedWidthReader(filePath, multiRecordColumnWidths);
				else
					reader = new FixedWidthReader(filePath, multiRecordColumnWidths.First().Value);
			}
			else
			{
				throw new ArgumentException("Unsupported file format");
			}
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

					if (reader != null)
					{
						reader.Dispose();
						reader = null;
					}
				}
			}
		}

		public bool MoveNext()
		{
			CheckDisposed();

			if (reader.EndOfFile)
				return false;

			var record = reader.Read();

			FieldMappingList mappings = null;

			if (recordFieldMappings.Count == 1)
				mappings = recordFieldMappings.First().Value;
			else
				mappings = recordFieldMappings[record[0]];

			var item = mappings.RecordType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

			ReadValues(item, mappings, record, 0);

			current = item;
			return true;
		}

		private void ReadValues(object item, FieldMappingList mappings, string[] record, int offset)
		{
			foreach (var field in mappings)
			{
				if (field.IsArray)
				{
					Array array = field.SetNewArray(item);

					for (int i = 0; i < field.ElementCount; i++)
					{
						if (field.IsComplexType)
						{
							var child = field.SetNewArrayInstance(array, i);

							ReadValues(child, field.InnerMappings, record, i * field.FieldCount);
						}
						else
						{
							array.SetValue(record[field.AbsoluteIndex + offset + i], i);
						}
					}
				}
				else if (field.IsComplexType)
				{
					var child = field.SetComplexType(item);

					ReadValues(child, field.InnerMappings, record, 0);
				}
				else
				{
					if (field.CanSetValue)
						field.SetValue(item, record[field.AbsoluteIndex + offset]);
				}
			}
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}
}
