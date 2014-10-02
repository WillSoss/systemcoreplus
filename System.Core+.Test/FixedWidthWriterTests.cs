﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;

namespace System.CorePlus.Test
{
	public abstract class fixed_width_writer_test : Specification
	{
		private MemoryStream stream;
		protected FixedWidthWriter writer;

		public fixed_width_writer_test()
		{
			stream = new MemoryStream();
			writer = new FixedWidthWriter(stream, 10);
		}

		public string File
		{
			get 
			{
				writer.Flush();
				return new StreamReader(new MemoryStream(stream.ToArray())).ReadToEnd(); 
			}
		}
	}

	public class fixed_width_writer : fixed_width_writer_test
	{
		public override void Observe()
		{
			writer.Write("asdf");
		}

		[Observation]
		public void should_pad_field()
		{
			var expected = "asdf      \r\n";
			var actual = File;
			StringBuilder debug = new StringBuilder();

			for (int i = 0; i < 10; i++)
			{
				debug.AppendLine("{0}{1} {2}{3}".FormatString(expected[i], (int)expected[i], actual[i], (int)actual[i]));
			}

			actual.ShouldEqual(expected);
		}
	}
}
