using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO.Parsing
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class FieldAttribute : Attribute
	{
		public int Index { get; private set; }
		public int Length { get; private set; }
		public Type ParserType { get; private set; }

		/// <summary>
		/// Creates a new instance of FieldAttribute
		/// </summary>
		/// <param name="index">The zero based position of the field</param>
		/// <param name="length">The length of the field</param>
		public FieldAttribute(int index, int length)
			: this(index, length, null)
		{
		}

		/// <summary>
		/// Creates a new instance of FieldAttribute
		/// </summary>
		/// <param name="index">The zero based position of the field</param>
		/// <param name="length">The length of the field</param>
		/// <param name="parserType">The type of parser to use for this field</param>
		public FieldAttribute(int index, int length, Type parserType)
		{
			this.Index = index;
			this.Length = length;
			this.ParserType = parserType;
		}
	}
}
