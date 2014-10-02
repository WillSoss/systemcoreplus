using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.IO.Parsing
{
	public interface IFieldParser
	{
		Type OutputType { get; }
		object Parse(string value);
	}

	public static class FieldParser
	{
		private static ReaderWriterLockSlim parsersLock;
		private static Dictionary<Type, IFieldParser> parsers;

		static FieldParser()
		{
			parsersLock = new ReaderWriterLockSlim();
			parsers = new Dictionary<Type, IFieldParser>();

			Register(new CharParser());
			Register(new StringParser());
			Register(new BooleanParser());
			Register(new Int32Parser());
			Register(new Int64Parser());
			Register(new DecimalParser());
		}

		public static IFieldParser Get(Type type)
		{
			parsersLock.EnterReadLock();

			try
			{
				return parsers[type];
			}
			finally
			{
				parsersLock.ExitReadLock();
			}
		}

		public static void Register(IFieldParser parser)
		{
			parsersLock.EnterWriteLock();

			try
			{
				if (parsers.ContainsKey(parser.OutputType))
					parsers.Remove(parser.OutputType);

				parsers.Add(parser.OutputType, parser);
			}
			finally
			{
				parsersLock.ExitWriteLock();
			}
		}
	}

	public abstract class FieldParser<T> : IFieldParser
	{
		public static implicit operator FieldParser<T>(Parse<T> parse)
		{
			return new DeferredParser<T>(parse);
		}

		public Type OutputType { get; private set; }

		public FieldParser()
		{
			this.OutputType = typeof(T);
		}

		object IFieldParser.Parse(string value)
		{
			return Parse(value);
		}

		public abstract T Parse(string value);
	}
}
