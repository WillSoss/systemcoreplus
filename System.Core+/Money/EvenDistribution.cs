using System;
using System.Collections.Generic;

namespace System
{
	public sealed class EvenDistribution : IEnumerable<decimal>
	{
		public decimal Dividend { get; private set; }
		public int Divisor { get; private set; }

		public bool IsDistributedEvenly { get; private set; }

		public decimal Quotient { get; private set; }
		public decimal Remainder { get; private set; }

		public decimal SmallerAmount { get { return Quotient; } }
		public int SmallerAmountCount { get; private set; }

		public decimal? LargerAmount { get; private set; }
		public int LargerAmountCount { get; private set; }

		internal EvenDistribution(decimal dividend, int divisor)
			: this(dividend, 2, divisor) { }

		internal EvenDistribution(decimal dividend, int dividendScale, int divisor)
		{
			if (divisor == 0)
				throw new DivideByZeroException();

			if (divisor < 1)
				throw new ArgumentOutOfRangeException("Divisor must be a positive number");

			dividend = Money.SetScale(dividend, dividendScale);

			this.Dividend = dividend;
			this.Divisor = divisor;

			PerformDivision();
		}

		private void PerformDivision()
		{
			int scale = Money.GetScale(Dividend);
			decimal unit = Money.GetSmallestAmount(scale);

			Quotient = decimal.Round(Dividend / Divisor, scale);
			Remainder = Dividend - (Quotient * Divisor);
			IsDistributedEvenly = Remainder == decimal.Zero;

			if (Remainder < decimal.Zero)
			{
				Quotient -= unit;
				Remainder = Dividend - (Quotient * Divisor);
			}

			LargerAmountCount = (int)(Remainder / unit);
			SmallerAmountCount = Divisor - LargerAmountCount;

			LargerAmount = LargerAmountCount == 0 ? (decimal?)null : Quotient + unit;

			if ((SmallerAmount * SmallerAmountCount) + (LargerAmount != null ? LargerAmount * LargerAmountCount : 0) != Dividend)
				throw new ApplicationException(string.Format("Could not divide {0} by {1}", Dividend, Divisor));
		}

		public IEnumerator<decimal> GetEnumerator()
		{
			for (int i = 0; i < SmallerAmountCount; i++)
				yield return SmallerAmount;

			for (int i = 0; i < LargerAmountCount; i++)
				yield return LargerAmount.Value;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
