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
		public bool IsComplexType { get { return InnerMappings != null; } }
		public int AbsoluteIndex { get; internal set; }
		public bool IsKeyField { get { return Index == 0; } }
		public bool CanSetValue { get; private set; }
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
			: this(fieldAttribute, (MemberInfo)memberInfo, memberInfo.PropertyType, memberInfo.CanWrite)
		{
			// If can't write and not key field (value supplied by class for record lookup when reading)
			if (!memberInfo.CanWrite && fieldAttribute.Index != 0)
				throw new FileParsingException(string.Format("Can not map property '{0}' because it is read-only", memberInfo.Name));
		}

		private FieldMapping(FieldAttribute fieldAttribute, MemberInfo memberInfo, Type type, bool canSetValue = true)
		{
			var inner = new FieldMappingList(type.IsArray ? type.GetElementType() : type);

			Index = fieldAttribute.Index;
			IsArray = fieldAttribute.Repeats > 0;
			ElementCount = fieldAttribute.Repeats > 0 ? fieldAttribute.Repeats : 1;
			FieldCount = inner.Count > 0 ? inner.Sum(m => m.FieldCount * m.ElementCount) : 1;
			FieldLength = inner.Count > 0 ? inner.Sum(m => m.FieldLength * m.ElementCount) : fieldAttribute.Length;
			InnerMappings = inner.Count > 0 ? inner : null;
			MemberInfo = memberInfo;
			MemberType = type.IsArray ? type.GetElementType() : type;
			CanSetValue = canSetValue;

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

		//internal FieldMapping(FieldArrayAttribute fieldArrayAttribute, PropertyInfo memberInfo)
		//	: this(fieldArrayAttribute, (MemberInfo)memberInfo, memberInfo.PropertyType)
		//{
		//	if (!memberInfo.CanWrite)
		//		throw new FileParsingException(string.Format("Can not map property '{0}' because it is read-only", memberInfo.Name));
		//}

		//private FieldMapping(FieldArrayAttribute fieldArrayAttribute, MemberInfo memberInfo, Type type)
		//{
		//	if (!type.IsArray)
		//		throw new FileParsingException(string.Format("Can not map property '{0}' because it is marked as an array but the type is not an array", memberInfo.Name));

		//	IsArray = true;
		//	InnerMappings = new FieldMappingList(type.GetElementType());
		//	Index = fieldArrayAttribute.Index;
		//	ElementCount = fieldArrayAttribute.Length;
		//	FieldCount = InnerMappings.Sum(m => m.FieldCount) * ElementCount;
		//	FieldLength = InnerMappings.Sum(m => m.FieldLength) * ElementCount;
		//	MemberInfo = memberInfo;
		//	MemberType = type.GetElementType();
		//	ArrayMemberType = type;
		//}

		internal IFieldParser GetParser()
		{
			var parser = Parser;

			if (parser == null)
			{
				try
				{
					parser = FieldParser.Get(IsArray ? MemberType.GetElementType() : MemberType);
				}
				catch (KeyNotFoundException ex)
				{
					throw new FileParsingException(string.Format("No parser is available for field or property '{0}'", MemberInfo.Name), ex);
				}	
			}

			return parser;
		}

		internal void SetValue(object item, string value)
		{
			object parsed = null;

			try
			{
				parsed = GetParser().Parse(value);
			}
			catch (FormatException ex)
			{
				throw new FormatException("Could not parse '{0}' into field '{1}'".FormatString(value, MemberType.Name), ex);
			}

			if (MemberInfo is PropertyInfo)
			{
				((PropertyInfo)MemberInfo).SetValue(item, parsed, null);
			}
			else
			{
				((FieldInfo)MemberInfo).SetValue(item, parsed);
			}
		}

		internal object SetComplexType(object item)
		{
			var ct = MemberType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

			if (MemberInfo is PropertyInfo)
			{
				((PropertyInfo)MemberInfo).SetValue(item, ct, null);
			}
			else
			{
				((FieldInfo)MemberInfo).SetValue(item, ct);
			}

			return ct;
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
