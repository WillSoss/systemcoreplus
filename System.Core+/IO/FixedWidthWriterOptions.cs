using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
	public enum ValueAlignment
	{
		Left = 0,
		Right = 1
	}

	public class FixedWidthWriterOptions
	{		
		/// <summary>
		/// Determines the character encoding used in the output
		/// </summary>
		public Encoding Encoding { get; set; }

		/// <summary>
		/// The character used to pad values so that the field length is met
		/// </summary>
		public char Padding { get; set; }

		/// <summary>
		/// The string placed between records, usually a line break string such as CR+LF, CR or LF
		/// </summary>
		public string NewLine { get; set; }

		/// <summary>
		/// Determines whether values that exceed the field length will be truncated or an exception will be thrown. False by default.
		/// </summary>
		public bool TruncateLongValues { get; set; }

		/// <summary>
		/// Determines whether values are aligned to the left with trailing padding or aligned on the right with leading padding. Left by default.
		/// </summary>
		public ValueAlignment ValueAlignment { get; set; }

		public FixedWidthWriterOptions()
		{
			Encoding = FixedWidthWriter.DefaultEncoding;
			Padding = FixedWidthWriter.DefaultPadding;
			NewLine = FixedWidthWriter.DefaultNewLine;
			TruncateLongValues = false;
			ValueAlignment = IO.ValueAlignment.Left;
		}
	}
}
