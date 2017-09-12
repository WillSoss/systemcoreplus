using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;

namespace System.CorePlus.Test.File_Format
{
	public abstract class MappingTest : Specification
	{
		string accountNumber = "1234567890";
		decimal amountDue = 12345.67M;
		bool isAdjudicated = true;
		string state = "LA";
		string street = "935 Gravier";

		int owner1Number = 34;
		string owner1Address = "837 Gravier";
		string owner1Name = "John Smith";
		string owner1Phone1 = "5041234567";
		string owner1Phone2 = "5049876543";

		int owner2Number = 78;
		string owner2Address = "1300 Perdido";
		string owner2Name = "Jane Smith";
		string owner2Phone1 = "9849485730";
		string owner2Phone2 = "4561237890";

		protected MemoryStream stream;
		protected FileFormatWriter writer;

		public MappingTest()
		{
			stream = new MemoryStream();

			writer = new FileFormatWriter(stream, IO.Parsing.FlatFileFormat.FixedWidth, typeof(PropertyModel));
		}

		protected PropertyModel GetModel()
		{
			return new PropertyModel()
			{
				AccountNumber = accountNumber,
				AmountDue = amountDue,
				IsAdjudicated = isAdjudicated,
				Address = new AddressModel()
				{
					State = state,
					Street = street
				},
				Owners = new OwnerModel[]
				{
					new OwnerModel()
					{
						Number = owner1Number,
						Name = owner1Name,
						Address = owner1Address,
						PhoneNumbers = new string[]
						{
							owner1Phone1,
							owner1Phone2
						}
					},
					new OwnerModel()
					{
						Number = owner2Number,
						Name = owner2Name,
						Address = owner2Address,
						PhoneNumbers = new string[]
						{
							owner2Phone1,
							owner2Phone2
						}
					}
				}
			};
		}
	}

	public class file_format_writer_writes_model_to_file : MappingTest
	{
		private string actualFile;
		private string expectedFile = "1234567890True 12345.67  34John Smith837 Gravier 5041234567504987654378Jane Smith1300 Perdido98494857304561237890935 Gravier LA";

		public override void Observe()
		{
			writer.Write(GetModel());
			writer.Flush();

			actualFile = Encoding.UTF8.GetString(stream.ToArray());

			// Remove byte order mark for test comparison
			// https://en.wikipedia.org/wiki/Byte_order_mark
			if (actualFile[0] == 0xFEFF)
				actualFile = actualFile.Substring(1, actualFile.Length - 1);
		}

		[Observation]
		public void file_should_be_created()
		{
			actualFile.Length.ShouldEqual(expectedFile.Length);
			actualFile.ShouldEqual(expectedFile);
		}
	}
}
