using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
	public interface IFlatFileReader : IDisposable
	{
		Stream BaseStream { get; }
		string[] Read();
	}
}
