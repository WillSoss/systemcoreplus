using System;
using System.IO;
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
}
