using System.IO.Parsing;

namespace System.CorePlus.Test.File_Format
{
	public class PropertyModel
	{
		[Field(0, 10)]
		public string AccountNumber { get; set; }
		[Field(1, 5)]
		public bool IsAdjudicated { get; set; }
		[Field(2, 10)]
		public decimal AmountDue { get; set; }
		[Field(3, repeats: 2)]
		public OwnerModel[] Owners { get; set; }
		[Field(4)]
		public AddressModel Address { get; set; }
	}

	public class AddressModel
	{
		[Field(0, 12)]
		public string Street { get; set; }
		[Field(1, 2)]
		public string State { get; set; }
	}

	public class OwnerModel
	{
		[Field(0, 2)]
		public int Number { get; set; }
		[Field(1, 10)]
		public string Name { get; set; }
		[Field(2, 12)]
		public string Address { get; set; }
		[Field(3, length: 10, repeats: 2)]
		public string[] PhoneNumbers { get; set; }
	}
}
