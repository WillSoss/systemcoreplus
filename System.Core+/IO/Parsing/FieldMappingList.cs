using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO.Parsing
{
	internal class FieldMappingList : IList<FieldMapping>
	{
		private List<FieldMapping> inner;

		public int Count
		{
			get { return inner.Count; }
		}

		public int FieldCount
		{
			get { return inner.Sum(m => m.FieldCount); }
		}

		public IEnumerable<int> FieldLenths
		{
			get
			{
				return GetFieldLengths(this);
			}
		}

		private IEnumerable<int> GetFieldLengths(FieldMappingList list)
		{
			foreach (var f in list)
			{
				if (f.IsArray)
				{
					// Repeat the inner field lengths for each element in the array
					for (int n = 0; n < f.ElementCount; n++)
					{
						foreach (var i in GetFieldLengths(f.InnerMappings))
							yield return i;
					}
				}
				else
				{
					yield return f.FieldLength;
				}
			}
		}

		public int Length
		{
			get { return inner.Sum(m => m.FieldLength); }
		}

		public int RecordLength
		{
			get { return inner.Sum(m => m.FieldLength); }
		}

		public FieldMapping this[int index]
		{
			get { return inner[index]; }
			set
			{
				CheckReadOnly();

				inner[index] = value;
			}
		}

		public bool IsReadOnly { get; private set; }

		public FieldMappingList(Type type)
		{
			inner = new List<FieldMapping>();
			this.IsReadOnly = false;

			var fields = type.GetFields();

			foreach (var field in fields)
			{
				var attrs = field.GetCustomAttributes(typeof(FieldArrayAttribute), true);

				if (attrs.Length > 0)
				{
					Add(new FieldMapping((FieldArrayAttribute)attrs.First(), field));
				}
				else
				{
					attrs = field.GetCustomAttributes(typeof(FieldAttribute), true);

					if (attrs.Length > 0)
						Add(new FieldMapping((FieldAttribute)attrs.First(), field));
				}
			}

			var props = type.GetProperties();

			foreach (var prop in props)
			{
				var attrs = prop.GetCustomAttributes(typeof(FieldArrayAttribute), true);

				if (attrs.Length > 0)
				{
					Add(new FieldMapping((FieldArrayAttribute)attrs.First(), prop));
				}
				else
				{
					attrs = prop.GetCustomAttributes(typeof(FieldAttribute), true);

					if (attrs.Length > 0)
						Add(new FieldMapping((FieldAttribute)attrs.First(), prop));
				}
			}

			Initialized();
		}

		internal void Initialized()
		{
			IsReadOnly = true;
			inner.Sort();

			int i = 0;
			foreach (var m in this)
			{
				if (m.Index != i)
					throw new FileParsingException(string.Format("Field attribute with index {0} not found. Field attributes must be unique and sequential, starting at zero.", i));

				i++;
			}
		}

		private void CheckReadOnly()
		{
			if (this.IsReadOnly)
				throw new InvalidOperationException("Cannot modify list, it is read-only");
		}

		public void Add(FieldMapping item)
		{
			CheckReadOnly();

			inner.Add(item);
		}

		public void Clear()
		{
			CheckReadOnly();

			inner.Clear();
		}

		public bool Contains(FieldMapping item)
		{
			return inner.Contains(item);
		}

		public void CopyTo(FieldMapping[] array, int arrayIndex)
		{
			inner.CopyTo(array, arrayIndex);
		}

		public IEnumerator<FieldMapping> GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int IndexOf(FieldMapping item)
		{
			return inner.IndexOf(item);
		}

		public void Insert(int index, FieldMapping item)
		{
			CheckReadOnly();

			inner.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			CheckReadOnly();

			inner.RemoveAt(index);
		}

		public bool Remove(FieldMapping item)
		{
			CheckReadOnly();

			return inner.Remove(item);
		}
	}
}
