using System;
using System.Collections.Generic;
using System.IO.Parsing;
using System.Linq;
using System.Text;

namespace System.IO.Parsing
{
	public enum FlatFileFormat
	{
		Csv = 0,
		FixedWidth = 1
	}

	public class FileParser<T> : IDisposable, IEnumerator<T>
		where T : new()
	{
		private IFlatFileReader reader;
		private T current;

		public bool IsDisposed { get; private set; }

		public T Current
		{
			get
			{
				CheckDisposed();

				return current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			get { return Current; }
		}

		public int FieldCount
		{
			get { return Mappings.FieldCount; }
		}

		public int RecordLength
		{
			get { return Mappings.RecordLength; }
		}

		internal int LongestRecordLength
		{
			get { return Mappings.Max(m => m.FieldLength); }
		}

		internal FieldMappingList Mappings { get; private set; }

		public FileParser(IFlatFileReader reader)
		{
			if (reader == null)
				throw new ArgumentException("Reader is required");

			this.IsDisposed = false;
			this.reader = reader;
			this.Mappings = new FieldMappingList(typeof(T));
		}

		public FileParser(string filePath, FlatFileFormat format)
			: this(GetReader(filePath, format)) { }

		private static IFlatFileReader GetReader(string filePath, FlatFileFormat format)
		{
			if (format == FlatFileFormat.Csv)
				return new CsvReader(filePath);

			if (format == FlatFileFormat.FixedWidth)
				return new FixedWidthReader(filePath, new FieldMappingList(typeof(T)).FieldLenths.ToArray());

			throw new ArgumentException("Unsupported file format");
		}

		public void SetParser(int index, IFieldParser parser)
		{
			if (index < 0 || index >= Mappings.Count)
				throw new IndexOutOfRangeException();

			Mappings.Single(m => m.Index == index).Parser = parser;
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
			T item = new T();

			ReadValues(item, Mappings, record, 0);

			current = item;
			return true;
		}

		private void ReadValues(object item, FieldMappingList mappings, string[] record, int baseIndex)
		{
			foreach (var field in mappings)
			{
				if (field.IsArray)
				{
					Array array = field.SetNewArray(item);

					for (int i = 0; i < field.ElementCount; i++)
					{
						var child = field.SetNewArrayInstance(array, i);

						// TODO: field.InnerMappings.Count needs to be replaced with a property that counts the fields for the mapping but does not multiple by the element count; an array in the array would break this
						ReadValues(child, field.InnerMappings, record, field.Index + baseIndex + (i * field.InnerMappings.Count));
					}
				}
				else
				{
					field.SetValue(item, record[field.Index + baseIndex]);
				}
			}
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}
}
