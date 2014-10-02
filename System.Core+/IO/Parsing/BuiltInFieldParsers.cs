using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO.Parsing
{
	public delegate T Parse<T>(string value);

	public class DeferredParser<T> : FieldParser<T>
	{
		private Parse<T> parse;

		public DeferredParser(Parse<T> parse)
		{
			if (parse == null)
				throw new ArgumentNullException("Parse is required");

			this.parse = parse;
		}

		public override T Parse(string value)
		{
			return parse(value);
		}
	}

	public class CharParser : FieldParser<Char>
	{
		public CharParser() { }

		public override char Parse(string value)
		{
			return char.Parse(value);
		}
	}

	public class StringParser : FieldParser<String>
	{
		public bool Trim { get; private set; }

		public StringParser()
			: this(true) { }

		public StringParser(bool trim)
		{
			this.Trim = trim;
		}

		public override string Parse(string value)
		{
			if (Trim)
				value = value.Trim();

			return value;
		}
	}

	public class BooleanParser : FieldParser<Boolean>
	{
		public BooleanParser() { }

		public override bool Parse(string value)
		{
			return Boolean.Parse(value);
		}
	}

	public class Int32Parser : FieldParser<Int32>
	{
		public Int32Parser() { }

		public override int Parse(string value)
		{
			return Int32.Parse(value);
		}
	}

	public class Int64Parser : FieldParser<Int64>
	{
		public Int64Parser() { }

		public override long Parse(string value)
		{
			return Int64.Parse(value);
		}
	}

	public class DecimalParser : FieldParser<Decimal>
	{
		public DecimalParser() { }

		public override decimal Parse(string value)
		{
			if (value.Trim().Equals(string.Empty))
				return 0M;

			return Decimal.Parse(value.Replace("$", ""));
		}
	}

	public class DateTimeParser : FieldParser<DateTime>
	{
		public DateTimeParser() { }

		public override DateTime Parse(string value)
		{
			return DateTime.Parse(value);
		}
	}
}
