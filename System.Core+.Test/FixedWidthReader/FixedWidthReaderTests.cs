using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Parsing;
using System.Linq;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test
{
	public class fixed_width_reader : Specification
    {
		FixedWidthReader r;

		public override void Observe()
		{
			r = new FixedWidthReader(@"Test Files\Fw1.txt", 15, 15, 4, 5, 10);
		}

        [Observation]
        public void should_read_fixed_width_file()
        {
			var data = r.Read();

			Assert.Equal(5, data.Length);
			Assert.Equal("First field", data[0]);
			Assert.Equal("Second field", data[1]);
			Assert.Equal("10.2", data[2]);
			Assert.Equal("true", data[3]);
			Assert.Equal("2010-01-01", data[4]);

			data = r.Read();

			Assert.Equal("First fi\r\neld", data[0]);

			data = r.Read();

			Assert.Null(data);
        }
    }

	public class fixed_width_file_with_short_record : Specification
	{
		FixedWidthReader reader;

		public override void Observe()
		{
			reader = new FixedWidthReader(@"Test Files\Fw2 - Short Record.txt", 15, 15, 4, 5, 10);
		}

		[Observation]
		public void should_throw_invalid_file_format_exception()
		{
			Assert.Throws<InvalidFileFormatException>(() => reader.Read());
		}
	}

	public class fixed_width_reader_multi_record : Specification
	{
		FixedWidthReader r;

		public override void Observe()
		{
			r = new FixedWidthReader(@"Test Files\Fw4.txt", new Dictionary<string, int[]> {
				{"0", new int[] {1, 6} },
				{"1", new int[] {1, 15} },
				{"2", new int[] {1, 24, 13 } }
			});
		}

		[Observation]
		public void should_read_fixed_width_file()
		{
			var results = r.ReadToEnd().ToArray();

			results[0][0].ShouldEqual("0");
			results[0][1].ShouldEqual("asdf");
			results[1][0].ShouldEqual("1");
			results[1][1].ShouldEqual("thisissometext");
			results[2][0].ShouldEqual("2");
			results[2][1].ShouldEqual("thisisanevenlongerrecord");
			results[2][2].ShouldEqual("withtwofields");
			results[3][0].ShouldEqual("0");
			results[3][1].ShouldEqual("1");
		}
	}

	public class Rec0
	{
		[Field(0, 1)]
		public int Type { get => 0; }

		[Field(1, 6)]
		public string Data1 { get; set; }
	}

	public class Rec1
	{
		[Field(0, 1)]
		public int Type { get => 1; }

		[Field(1, 15)]
		public string Data1 { get; set; }
	}

	public class Rec2
	{
		[Field(0, 1)]
		public int Type { get => 2; }

		[Field(1, 24)]
		public string Data1 { get; set; }

		[Field(2, 13)]
		public string Data2 { get; set; }
	}

	public class file_format_reader_tests : Specification
	{
		FileFormatReader reader;
		List<object> records = new List<object>();

		public override void Observe()
		{
			reader = new FileFormatReader(@"Test Files\Fw4.txt", FlatFileFormat.FixedWidth, typeof(Rec0), typeof(Rec1), typeof(Rec2));

			while (reader.MoveNext())
			{
				records.Add(reader.Current);
			}
		}

		[Observation]
		public void should_read_four_records()
		{
			records.Count.ShouldEqual(4);
		}

		[Observation]
		public void should_read_file()
		{
			records[0].ShouldBeType<Rec0>();
			((Rec0)records[0]).Data1.ShouldEqual("asdf");

			records[1].ShouldBeType<Rec1>();
			((Rec1)records[1]).Data1.ShouldEqual("thisissometext");

			records[2].ShouldBeType<Rec2>();
			((Rec2)records[2]).Data1.ShouldEqual("thisisanevenlongerrecord");
			((Rec2)records[2]).Data2.ShouldEqual("withtwofields");
			
			records[3].ShouldBeType<Rec0>();
			((Rec0)records[3]).Data1.ShouldEqual("1");
		}
	}
}
