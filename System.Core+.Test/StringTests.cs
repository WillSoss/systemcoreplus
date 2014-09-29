using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.CorePlus.Test
{
	[TestClass]
	public class StringTests
	{
		string nullstring = null;
		string asdf = " asdf ";
		string consecWhitespace = " a  s \t\t d\r\n\t f ";

		[TestMethod]
		public void TruncateTest()
		{
			Assert.IsNull(nullstring.Truncate(10));
			Assert.AreEqual(asdf, asdf.Truncate(10));
			Assert.AreEqual(" a", asdf.Truncate(2));
		}

		[TestMethod]
		public void TrimNullSafeTest()
		{
			Assert.IsNull(nullstring.TrimNullSafe());
			Assert.AreEqual("asdf", asdf.TrimNullSafe());
		}

		[TestMethod]
		public void TrimWhitespaceTest()
		{
			Assert.AreEqual("a s d\r\n f", consecWhitespace.TrimWhitespace());
			Assert.AreEqual("a_s_d\r\n_f", consecWhitespace.TrimWhitespace("_"));
			Assert.AreEqual("a s d f", consecWhitespace.TrimWhitespace(true));
			Assert.AreEqual("a_s_d_f", consecWhitespace.TrimWhitespace("_", true));
		}

		[TestMethod]
		public void FormatTest()
		{
			var format = "{0:mmHHss}.{1:C}";
			var provider = new CultureInfo("en-US", true);
			var arg0 = DateTime.Now;
			var arg1 = 123.45M;

			Assert.AreEqual(string.Format(format, new object[] { arg0, arg1 }), format.FormatString(arg0, arg1));
			Assert.AreEqual(string.Format(provider, format, arg0, arg1), format.FormatString(provider, arg0, arg1));
		}
	}
}
