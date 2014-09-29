using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    public class CsvWriter : IDisposable
    {
        public const char DefaultQualifier = '"';
        public const char DefaultDelimiter = ',';
        public static readonly string DefaultNewLine = Environment.NewLine;

        private StreamWriter writer;
        private volatile bool disposed = false;

        private char qualifier;
        private char delimiter;
        private string newLine;
        private string escapedQualifier;
        private char[] qualifierNeeded = new char[] 
        {
            DefaultQualifier, DefaultDelimiter,
            '\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', '\u0006', '\u0007', 
            '\u0008', '\u0009', '\u000A', '\u000B', '\u000C', '\u000D', '\u000E', '\u000F',
            '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017',
            '\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', '\u001E', '\u001F', '\u007F'
        };

        public CsvWriter(string filePath)
            : this(File.Open(filePath, FileMode.CreateNew)) { }

        public CsvWriter(string filePath, FileMode mode)
            : this(File.Open(filePath, mode)) { }

		public CsvWriter(Stream s)
			: this(s, DefaultQualifier, DefaultDelimiter) { }

        public CsvWriter(Stream s, char qualifier, char delimiter)
            : this(s, qualifier, delimiter, Encoding.UTF8) { }

        public CsvWriter(Stream s, char qualifier, char delimiter, string newLine)
            : this(s, qualifier, delimiter, newLine, Encoding.UTF8) { }

        public CsvWriter(Stream s, char qualifier, char delimiter, Encoding encoding)
            : this(s, qualifier, delimiter, DefaultNewLine, encoding) { }

        public CsvWriter(Stream s, char qualifier, char delimiter, string newLine, Encoding encoding)
        {
            this.writer = new StreamWriter(s, encoding);
            this.delimiter = delimiter;
            this.qualifier = qualifier;
            this.newLine = newLine;
            this.escapedQualifier = new string(qualifier, 2);
            this.qualifierNeeded[0] = qualifier;
            this.qualifierNeeded[1] = delimiter;
        }

        public Stream BaseStream
        {
            get { return writer.BaseStream; }
        }

        public void Write(params string[] record)
        {
            Write((IEnumerable)record);
        }

        public void Write(params object[] record)
        {
            Write((IEnumerable)record);
        }

        public void Write(IEnumerable record)
        {
            bool first = true;

            foreach (var obj in record)
            {
                if (first)
                    first = false;
                else
                    writer.Write(delimiter);

                var field = (obj ?? string.Empty).ToString();

                if (field.IndexOfAny(qualifierNeeded) > -1)
                {
                    field = qualifier + field.Replace(qualifier.ToString(), escapedQualifier) + qualifier;
                }

                writer.Write(field);
            }

            if (first)
                throw new ArgumentException("There are zero fields in the record to write");
            else
                writer.Write(newLine);
        }
        
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;

                if (this.writer != null)
                    this.writer.Dispose();
            }
        }
    }
}
