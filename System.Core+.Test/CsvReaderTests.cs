using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.CorePlus.Test
{
    [TestClass]
    public class CsvReaderTests
    {
        [TestMethod]
        public void ReadCsv()
        {
            using (var r = new CsvReader(@"Test Files\Csv\Csv1.csv"))
            {
                var data = r.Read();

                Assert.AreEqual(6, data.Length);
                Assert.AreEqual("\"", data[0], "Qualifier in field");
                Assert.AreEqual(",", data[1], "Delimiter in field");
                Assert.AreEqual("hello", data[2], "Unqualified field");
                Assert.AreEqual("world", data[3], "Unqualified field");
                Assert.AreEqual("hello,world", data[4], "Delimiter in field");
                Assert.AreEqual(@"line
break", data[5], "Line break in field");
            }
        }
    }
}
