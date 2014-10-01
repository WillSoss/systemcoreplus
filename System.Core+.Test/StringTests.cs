using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test
{
	public class truncate
	{
		string nullstring = null;
		string asdf = " asdf ";

		[Observation]
		public void should_truncate_strings()
		{
			Assert.Null(nullstring.Truncate(10));
			Assert.Equal(asdf, asdf.Truncate(10));
			Assert.Equal(" a", asdf.Truncate(2));
		}
	}

	public class trim_null_safe
	{
		string nullstring = null;
		string asdf = " asdf ";

		[Observation]
		public void should_trim_null_string()
		{
			Assert.Null(nullstring.TrimNullSafe());
		}

		[Observation]
		public void should_trim_non_null_string()
		{
			Assert.Equal("asdf", asdf.TrimNullSafe());
		}
	}

	public class trim_whitespace
	{

		string consecWhitespace = " a  s \t\t d\r\n\t f ";

		[Observation]
		public void should_trim_whitespace()
		{
			Assert.Equal("a s d\r\n f", consecWhitespace.TrimWhitespace());
			Assert.Equal("a_s_d\r\n_f", consecWhitespace.TrimWhitespace("_"));
			Assert.Equal("a s d f", consecWhitespace.TrimWhitespace(true));
			Assert.Equal("a_s_d_f", consecWhitespace.TrimWhitespace("_", true));
		}
	}

	public class format_string
	{
		[Observation]
		public void should_format_strings()
		{
			var format = "{0:mmHHss}.{1:C}";
			var provider = new CultureInfo("en-US", true);
			var arg0 = DateTime.Now;
			var arg1 = 123.45M;

			Assert.Equal(string.Format(format, new object[] { arg0, arg1 }), format.FormatString(arg0, arg1));
			Assert.Equal(string.Format(provider, format, arg0, arg1), format.FormatString(provider, arg0, arg1));
		}
	}
}
