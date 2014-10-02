using System;
using System.Collections;

namespace System.IO
{
	public interface IFlatFileWriter : IDisposable
	{
		Stream BaseStream { get; }

		void Write(params string[] record);
		void Write(params object[] record);
		void Write(IEnumerable record);

		void Flush();
	}
}
