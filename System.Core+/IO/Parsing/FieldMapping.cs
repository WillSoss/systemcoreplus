using System;
using System.Collections.Generic;
using System.IO.Parsing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.IO.Parsing
{
	public class FieldMapping : IComparable<FieldMapping>
	{
		private IFieldParser parser;

		public int Index { get; private set; }
		public int ElementCount { get; private set; }
		public int FieldCount { get; private set; }
		public int FieldLength { get; private set; }
		public bool IsArray { get; private set; }
		internal FieldMappingList InnerMappings { get; private set; }

		public IFieldParser Parser
		{
			get { return parser; }
			set
			{
				if (!MemberType.Equals(value.OutputType) && !MemberType.IsAssignableFrom(value.OutputType))
					throw new FileParsingException(string.Format("Cannot use parser of type '{0}' for field or property '{1}' because the parser output type ({2}) does not match the field or property type ({3})", value.GetType().Name, MemberInfo.Name, value.OutputType.FullName, MemberType.FullName));

				parser = value;
			}
		}

		internal MemberInfo MemberInfo { get; private set; }
		internal Type MemberType { get; private set; }
		internal Type ArrayMemberType { get; private set; }

		internal FieldMapping(FieldAttribute fieldAttribute, FieldInfo memberInfo)
			: this(fieldAttribute, (MemberInfo)memberInfo, memberInfo.FieldType)
		{
		}

		internal FieldMapping(FieldAttribute fieldAttribute, PropertyInfo memberInfo)
			: this(fieldAttribute, (MemberInfo)memberInfo, memberInfo.PropertyType)
		{
			if (!memberInfo.CanWrite)
				throw new FileParsingException(string.Format("Can not map property '{0}' because it is read-only", memberInfo.Name));
		}

		private FieldMapping(FieldAttribute fieldAttribute, MemberInfo memberInfo, Type type)
		{
			Index = fieldAttribute.Index;
			ElementCount = 1;
			FieldCount = 1;
			FieldLength = fieldAttribute.Length;
			IsArray = false;
			InnerMappings = null;
			MemberInfo = memberInfo;
			MemberType = type;

			if (fieldAttribute.ParserType != null)
			{
				if (!typeof(IFieldParser).IsAssignableFrom(fieldAttribute.ParserType))
					throw new FileParsingException(string.Format("Cannot instantiate parser of type '{0}' on field or property '{1}' because the type specified does not implement Archon.FileParsing.IParser", fieldAttribute.ParserType.Name, memberInfo.Name));

				var constructor = fieldAttribute.ParserType.GetConstructor(Type.EmptyTypes);

				if (constructor == null)
					throw new FileParsingException(string.Format("Cannot instantiate parser of type '{0}' on field or property '{1}' because the type does not have a default constructor", fieldAttribute.ParserType.Name, memberInfo.Name));

				Parser = (IFieldParser)constructor.Invoke(new object[0]);
			}
		}

		internal FieldMapping(FieldArrayAttribute fieldArrayAttribute, FieldInfo memberInfo)
			: this(fieldArrayAttribute, (MemberInfo)memberInfo, memberInfo.FieldType)
		{

		}

		internal FieldMapping(FieldArrayAttribute fieldArrayAttribute, PropertyInfo memberInfo)
			: this(fieldArrayAttribute, (MemberInfo)memberInfo, memberInfo.PropertyType)
		{
			if (!memberInfo.CanWrite)
				throw new FileParsingException(string.Format("Can not map property '{0}' because it is read-only", memberInfo.Name));
		}

		private FieldMapping(FieldArrayAttribute fieldArrayAttribute, MemberInfo memberInfo, Type type)
		{
			if (!type.IsArray)
				throw new FileParsingException(string.Format("Can not map property '{0}' because it is marked as an array but the type is not an array", memberInfo.Name));

			IsArray = true;
			InnerMappings = new FieldMappingList(type.GetElementType());
			Index = fieldArrayAttribute.Index;
			ElementCount = fieldArrayAttribute.Length;
			FieldCount = InnerMappings.Sum(m => m.FieldCount) * ElementCount;
			FieldLength = InnerMappings.Sum(m => m.FieldLength) * ElementCount;
			MemberInfo = memberInfo;
			MemberType = type.GetElementType();
			ArrayMemberType = type;
		}

		internal IFieldParser GetParser()
		{
			var parser = Parser;

			if (parser == null)
			{
				parser = FieldParser.Get(MemberType);

				if (parser == null)
					throw new FileParsingException(string.Format("No parser is available for field or property '{0}'", MemberInfo.Name));
			}

			return parser;
		}

		internal void SetValue(object item, string value)
		{
			object parsed = GetParser().Parse(value);

			if (MemberInfo is PropertyInfo)
			{
				((PropertyInfo)MemberInfo).SetValue(item, parsed, null);
			}
			else
			{
				((FieldInfo)MemberInfo).SetValue(item, parsed);
			}
		}

		internal Array SetNewArray(object item)
		{
			if (!IsArray)
				throw new InvalidOperationException();

			Array array = Array.CreateInstance(MemberType, ElementCount);

			if (MemberInfo is PropertyInfo)
			{
				((PropertyInfo)MemberInfo).SetValue(item, array, null);
			}
			else
			{
				((FieldInfo)MemberInfo).SetValue(item, array);
			}

			return array;
		}

		internal object SetNewArrayInstance(Array array, int index)
		{
			object item = MemberType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

			array.SetValue(item, index);

			return item;
		}

		public int CompareTo(FieldMapping other)
		{
			return this.Index.CompareTo(other.Index);
		}
	}
}
