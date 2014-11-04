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
		public int Repeats { get; private set; }
		public Type ParserType { get; private set; }

		/// <summary>
		/// Creates a new instance of FieldAttribute
		/// </summary>
		/// <param name="index">The zero based position of the field</param>
		/// <param name="length">The length of the field</param>
		public FieldAttribute(int index, int length = 0, int repeats = 0)
			: this(index, length, null)
		{
			if (repeats < 2 && repeats != 0)
				throw new ArgumentOutOfRangeException("Repeats must either be zero or greater than or equal to two");

			this.Repeats = repeats;
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
