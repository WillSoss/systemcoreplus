using System.Collections.Generic;
using System.IO.Parsing;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test.File_Format
{
	public class parsing_fixed_width_file : Specification
	{
		List<PropertyModel> properties;

		public override void Observe()
		{
			properties = new List<PropertyModel>();

			using (var parser = new FileParser<PropertyModel>(@"..\..\Test Files\Fw3.txt", FlatFileFormat.FixedWidth))
			{
				while (parser.MoveNext())
					properties.Add(parser.Current);
			}
		}

		[Observation]
		public void should_parse_file()
		{
			properties.Count.ShouldEqual(2);

			properties[0].AccountNumber.ShouldEqual("12345678");
			properties[0].IsAdjudicated.ShouldBeFalse();
			properties[0].AmountDue.ShouldEqual(123.45M);
			properties[0].Owners[0].Number.ShouldEqual(12);
			properties[0].Owners[0].Name.ShouldEqual("John Smith");
			properties[0].Owners[0].Address.ShouldEqual("123 Main St");
			properties[0].Owners[1].Number.ShouldEqual(13);
			properties[0].Owners[1].Name.ShouldEqual("Jane Smith");
			properties[0].Owners[1].Address.ShouldEqual("124 Main St");

			properties[1].AccountNumber.ShouldEqual("12324589");
			properties[1].IsAdjudicated.ShouldBeTrue();
			properties[1].AmountDue.ShouldEqual(3094.98M);
			properties[1].Owners[0].Number.ShouldEqual(12);
			properties[1].Owners[0].Name.ShouldEqual("John Smith");
			properties[1].Owners[0].Address.ShouldEqual("123 Main St");
			properties[1].Owners[1].Number.ShouldEqual(13);
			properties[1].Owners[1].Name.ShouldEqual("Jane Smith");
			properties[1].Owners[1].Address.ShouldEqual("124 Main St");
		}

		[Fact(Skip = "Not really a test")]
		public void PrintFieldMappings()
		{
			FieldMappingList list = new FieldMappingList(typeof(PropertyModel));
			StringBuilder sb = new StringBuilder();

			print(list, 0, sb);

			Assert.Equal(sb.ToString(), "");
		}

		private void print(FieldMappingList list, int depth, StringBuilder sb)
		{
			foreach (var f in list)
			{
				sb.Append(new string(' ', depth));
				sb.Append(f.AbsoluteIndex + ": ");
				sb.Append(f.MemberInfo.Name);

				if (f.IsArray)
					sb.Append("[" + f.ElementCount + "]");

				sb.AppendLine(" " + f.FieldCount + "x" + f.FieldLength);

				if (f.InnerMappings != null)
					print(f.InnerMappings, depth + 2, sb);
			}
		}
	}
}
