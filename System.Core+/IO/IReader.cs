using System;
using System.IO;

namespace System.IO
{
	internal interface IReader : IDisposable
	{
		int Read();
		int Peek();
		bool EndOfStream { get; }
		Stream BaseStream { get; }
	}
}
