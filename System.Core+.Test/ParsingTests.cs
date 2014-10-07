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
		[Field(2, 10)]
		public string Address { get; set; }
	}

	public class parsing_fixed_width_file : Specification
	{
		public override void Observe()
		{
			using (var parser = new FileParser<PropertyModel>("", FlatFileFormat.FixedWidth))
			{

			}
		}

		[Observation]
		public void should_parse_file()
		{
			throw new NotImplementedException();
		}
	}
}
