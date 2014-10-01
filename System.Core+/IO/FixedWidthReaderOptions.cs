using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
	public class FixedWidthReaderOptions
	{
		public Encoding Encoding { get; set; }
		public char[] Padding { get; set; }

		public FixedWidthReaderOptions()
		{
			Encoding = Encoding.UTF8;
			Padding = new char[] { ' ', '\t' };
		}
	}
}
