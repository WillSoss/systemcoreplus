using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Extensions;

namespace System.CorePlus.Test.File_Format
{
	public abstract class FileFormatWriterTest : Specification
	{
		string accountNumber = "1234567890";
		decimal amountDue = 12345.67M;
		bool isAdjudicated = true;
		string state = "LA";
		string street = "935 Gravier St";

		int owner1Number = 342;
		string owner1Address = "837 Gravier St";
		string owner1Name = "John Smith";
		string owner1Phone1 = "5041234567";
		string owner1Phone2 = "5049876543";

		int owner2Number = 7800;
		string owner2Address = "1300 Perdido St";
		string owner2Name = "Jane Smith";
		string owner2Phone1 = "9849485730";
		string owner2Phone2 = "4561237890";

		protected MemoryStream stream;
		protected FileFormatWriter writer;

		public FileFormatWriterTest()
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

	public class file_format_writer_writes_model_to_file : FileFormatWriterTest
	{
		private string file;

		public override void Observe()
		{
			writer.Write(GetModel());

			file = Encoding.UTF8.GetString(stream.ToArray());
		}

		[Observation]
		public void file_should_be_created()
		{
			file.ShouldEqual("");
		}
	}
}
