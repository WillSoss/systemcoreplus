using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test
{
	public class PropertyModel
	{
		[Field(0, 10)]
		public string AccountNumber { get; set; }
		[Field(1, 5)]
		public bool IsAdjudicated { get; set; }
		[Field(2, 10)]
		public decimal AmountDue { get; set; }
		[FieldArray(3, 2)]
		public OwnerModel[] Owners { get; set; }
	}

	public class OwnerModel
	{
		[Field(0, 2)]
		public int Number { get; set; }
		[Field(1, 10)]
		public string Name { get; set; }
		[Field(2, 12)]
		public string Address { get; set; }
	}

	public class parsing_fixed_width_file : Specification
	{
		List<PropertyModel> properties;

		public override void Observe()
		{
			properties = new List<PropertyModel>();

			using (var parser = new FileParser<PropertyModel>(@"Test Files\Fw3.txt", FlatFileFormat.FixedWidth))
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
	}
}
