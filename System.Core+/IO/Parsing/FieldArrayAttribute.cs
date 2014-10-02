using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO.Parsing
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class FieldArrayAttribute : FieldAttribute
	{
		/// <summary>
		/// Creates a new instance of FieldArrayAttribute
		/// </summary>
		/// <param name="index">The zero based position of the field</param>
		/// <param name="length">The number of elements in the array</param>
		public FieldArrayAttribute(int index, int length)
			: base(index, length, null)
		{
		}
	}
}
