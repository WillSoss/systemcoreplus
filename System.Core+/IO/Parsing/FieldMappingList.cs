using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		public IEnumerable<int> FieldLengths
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
						if (f.IsComplexType)
						{
							foreach (var i in GetFieldLengths(f.InnerMappings))
								yield return i;
						}
						else
						{
							yield return f.FieldLength;
						}
					}
				}
				else if (f.IsComplexType)
				{
					foreach (var i in GetFieldLengths(f.InnerMappings))
						yield return i;
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

		/// <summary>
		/// When reading a file format with multiple record types the key value is used to determine the type of record being read.
		/// </summary>
		public string Key { get; private set; }

		public Type RecordType { get; private set; }

		public FieldMappingList(Type type)
		{
			inner = new List<FieldMapping>();
			this.IsReadOnly = false;
			this.RecordType = type;

			// Check fields for attribute

			var fields = type.GetFields();

			foreach (var field in fields)
			{
				var attrs = field.GetCustomAttributes(typeof(FieldAttribute), true);

				if (attrs.Length > 0)
					Add(new FieldMapping((FieldAttribute)attrs.First(), field));
			}

			// Check properties for attribute

			var props = type.GetProperties();

			foreach (var prop in props)
			{
				var attrs = prop.GetCustomAttributes(typeof(FieldAttribute), true);

				if (attrs.Length > 0)
					Add(new FieldMapping((FieldAttribute)attrs.First(), prop));
			}

			IsReadOnly = true;
			inner.Sort();

			int i = 0;
			foreach (var m in this)
			{
				if (m.Index != i)
					throw new FileParsingException(string.Format("Field attribute with index {0} not found. Field attributes must be unique and sequential, starting at zero.", i));

				i++;
			}

			if (inner.Count > 0)
			{
				// Get record key value
				var cons = type.GetConstructor(Type.EmptyTypes);

				if (cons == null)
					throw new ArgumentException($"The record type {type.FullName} does not have a default constructor");

				var rec = cons.Invoke(new object[0]);
				var def = inner[0];

				if (def.MemberInfo is PropertyInfo)
				{
					Key = ((PropertyInfo)def.MemberInfo).GetValue(rec, null).ToString();
				}
				else
				{
					Key = ((FieldInfo)def.MemberInfo).GetValue(rec).ToString();
				}
			}

			ComputeAbsoluteIndexes(0);
		}

		private void ComputeAbsoluteIndexes(int baseIndex)
		{
			int i = baseIndex;
			foreach (var m in this)
			{
				m.AbsoluteIndex = i;

				if (m.IsComplexType)
					m.InnerMappings.ComputeAbsoluteIndexes(i);

				i += m.ElementCount * m.FieldCount;
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
