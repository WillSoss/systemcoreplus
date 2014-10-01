using System;
using System.IO;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test
{
    public class csv_reader
    {
        [Observation]
        public void should_read_csv()
        {
            using (var r = new CsvReader(@"Test Files\Csv1.csv"))
            {
                var data = r.Read();

                Assert.Equal(6, data.Length);
                Assert.Equal("\"", data[0]);
                Assert.Equal(",", data[1]);
                Assert.Equal("hello", data[2]);
                Assert.Equal("world", data[3]);
                Assert.Equal("hello,world", data[4]);
                Assert.Equal(@"line
break", data[5]);
            }
        }
    }
}
