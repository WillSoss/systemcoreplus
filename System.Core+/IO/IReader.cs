using System;
using System.IO;

namespace System.IO
{
	internal interface IReader : IDisposable
	{
		int Read();
		string Read(int length);
		int Peek();
		bool EndOfStream { get; }
		Stream BaseStream { get; }
	}
}
