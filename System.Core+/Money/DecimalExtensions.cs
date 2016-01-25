using System;
using System.Collections.Generic;
using System.Linq;

namespace System
{
	public static class DecimalExtensions
	{
		/// <summary>
		/// Returns the negated value if the value is less than zero, otherwise returns the value unchanged
		/// </summary>
		public static decimal NonNegative(this decimal value)
		{
			return value < decimal.Zero ? decimal.Negate(value) : value;
		}

		public static int GetScale(this decimal value)
		{
			return Money.GetScale(value);
		}

		public static int GetSignificantDigitCount(this decimal value)
		{
			return Money.GetSignificantDigitCount(value);
		}
	}
}
