using System;
using System.Collections.Generic;
using System.Globalization;

namespace System
{
	public static class Money
	{
		public const byte DefaultScale = 2;
		public const byte MaxScale = 12;

		/// <summary>
		/// Divides a monetary amount into a number of stacks (or accounts)
		/// </summary>
		/// <param name="dividend">The amount to divide</param>
		/// <param name="divisor">The number of stacks to divide the money into</param>		
		public static EvenDistribution DistributeEvenly(this decimal dividend, int divisor)
		{
			return new EvenDistribution(dividend, divisor);
		}

		/// <summary>
		/// Divides a monetary amount into a number of stacks (or accounts)
		/// </summary>
		/// <param name="dividend">The amount to divide</param>
		/// <param name="divisor">The number of stacks to divide the money into</param>
		/// <param name="scale">The number of places to the right of the decimal to calculate</param>
		public static EvenDistribution DistributeEvenly(this decimal dividend, int divisor, int scale)
		{
			return new EvenDistribution(dividend, scale, divisor);
		}

        /// <summary>
        /// Performs a pro-rata, or weighted, distribution of the total amount specified.
        /// </summary>
        /// <param name="total">The total amount to distribute</param>
        /// <param name="weights">A list of weights to determine the proportion of the total allocated</param>
        /// <param name="scale">The number of decimal places in the amounts distributed, 2 by default</param>
        /// <param name="remainderGoesTo">Determines how to treat the remainder</param>
        /// <returns>A list of ProRataShares identifying the amount allocated to each weight</returns>
        public static ProRataDistribution DistributeProRata(this decimal total, decimal[] weights, int scale = 2, RemainderGoesTo remainderGoesTo = RemainderGoesTo.SmallestAmounts, bool takeNegativeRemainderFromSame = true)
        {
            return new ProRataDistribution(total, weights, scale, remainderGoesTo, takeNegativeRemainderFromSame);
        }

        /// <summary>
        /// Performs a pro-rata, or weighted, distribution of the total amount specified.
        /// </summary>
        /// <typeparam name="T">The type of identifier for weights</typeparam>
        /// <param name="total">The total amount to distribute</param>
        /// <param name="weights">A dictionary of weights (values) with unique identifiers (keys)</param>
        /// <param name="scale">The number of decimal places in the amounts distributed, 2 by default</param>
        /// /// <param name="remainderGoesTo">Determines how to treat the remainder</param>
        /// <returns>A list of ProRataShares identifying the amount allocated to each weight</returns>
        public static ProRataDistribution<T> DistributeProRata<T>(this decimal total, Dictionary<T, decimal> weights, int scale = 2, RemainderGoesTo remainderGoesTo = RemainderGoesTo.SmallestAmounts, bool takeNegativeRemainderFromSame = true)
        {
            return new ProRataDistribution<T>(total, weights, scale, remainderGoesTo, takeNegativeRemainderFromSame);
        }

        public static string ToMoneyString(this decimal value)
        {
            return ToMoneyString(value, 2);
        }

		public static string ToMoneyString(this decimal value, int scale)
		{
            if (scale < 0)
                scale = GetScale(value);

			return value.ToString(string.Format("C{0}", scale));
		}

		public static decimal Parse(string s)
		{
			if (String.IsNullOrEmpty(s)) throw new ArgumentNullException("s");

			decimal m;
			if (!TryParse(s, out m))
				throw new FormatException(String.Format("The specified string was not a valid currency string: {0}", s));

			return m;
		}

		public static bool TryParse(string s, out decimal m)
		{
			m = decimal.Zero;

			if (String.IsNullOrEmpty(s))
				return false;

			bool isNegated = false;

			if (s.StartsWith("(") && s.EndsWith(")"))
			{
				s = s.Remove(0, 1);
				s = s.Remove(s.Length - 1, 1);
				isNegated = true;
			}

			if (s.StartsWith("-"))
			{
				s = s.Remove(0, 1);
				isNegated = true;
			}

			string currencySymbol = NumberFormatInfo.CurrentInfo.CurrencySymbol;
			if (s.StartsWith(currencySymbol))
			{
				s = s.Remove(0, currencySymbol.Length);
			}

			decimal d;
			if (!decimal.TryParse(s, out d))
				return false;

			d = isNegated ? -d : d;

			return true;
		}
		
		/// <summary>
		/// Gets the smallest amount of money that the indicated scale allows, e.g., the smallest amount at a scale of 2 would be $0.01 and a scale of 8 would be $0.00000001
		/// </summary>
		/// <returns>The smallest amount of money that the indicated scale allows</returns>
		public static decimal GetSmallestAmount(int scale)
		{
			ValidateScale(scale);

			return SetScale(10M / (decimal)Math.Pow(10, scale + 1), scale);
		}

        public static decimal GetNumberOfSmallestAmountsInOne(int scale)
        {
            return (int)1M / GetSmallestAmount(scale);
        }

		/// <summary>
		/// Gets the number of significant digits to the right of the decimal place, which is the minimum scale that a Money instance can use without truncating or rounding
		/// </summary>
		/// <param name="value">The value to inspect</param>
		/// <returns>The number of significant digits to the right of the decimal place</returns>
		public static int GetSignificantDigitCount(decimal value)
		{
			var str = value.ToString();
			var dec = str.IndexOf('.');

			if (dec == -1)
				return 0;

			str = str.Substring(dec + 1, str.Length - dec - 1).TrimEnd('0');

			return str.Length;
		}

		/// <summary>
		/// Gets the scale of the decimal value
		/// </summary>
		/// <returns>An integer indicating the scale of the decimal</returns>
		public static int GetScale(decimal value)
		{
			return (decimal.GetBits(value)[3] & 0xFF0000) >> 16;
		}

		/// <summary>
		/// Sets the scale of the decimal value to the specified scale, leaving the value of the decimal unchanged
		/// </summary>
		/// <returns>The same decimal value with the scale specified</returns>
		public static decimal SetScale(decimal value, int scale)
		{
			ValidateScale(scale);
			ValidateScaleAccomodatesSignificantDigits(value, scale);
			var actualScale = GetScale(value);

			if (value == decimal.Zero)
			{
				// Special case where other techniques don't work
				return new decimal(0, 0, 0, false, (byte)scale);
			}
			else if (actualScale == scale)
			{
				return value;
			}
			else if (actualScale > scale)
			{
				// Decrease scale by rounding off trailing zeros
				return decimal.Round(value, scale);
			}
			else
			{
				// Increase scale by multiplying by 1 with desired scale
				var one = new decimal(0, 0, 0, false, (byte)scale) + 1M;
				return decimal.Round(value * one, scale);
			}
		}

		private static void ValidateScale(byte scale)
		{
			ValidateScale((int)scale);
		}

		private static void ValidateScale(int scale)
		{
			if (scale < 0)
				throw new ArgumentOutOfRangeException("Scale must be a non-negative value");

			if (scale > MaxScale)
				throw new ArgumentOutOfRangeException(string.Format("Scale cannot be larger than {0}", MaxScale));
		}

		private static void ValidateScaleAccomodatesSignificantDigits(decimal value, int scale)
		{
			if (scale < GetSignificantDigitCount(value))
				throw new OverflowException("The specified scale is less than the number of significant digits to the right of the decimal place, which would result in truncating the value");
		}
	}
}
