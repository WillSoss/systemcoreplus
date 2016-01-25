using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class ProRataDistribution : ProRataDistribution<int>
    {
        internal ProRataDistribution(decimal total, decimal[] weights, int scale, RemainderGoesTo remainderGoesTo, bool takeNegativeRemainderFromSame)
            : base(total, weights.Select((v, i) => new { v, i }).ToDictionary(wi => wi.i, wv => wv.v), scale, remainderGoesTo, takeNegativeRemainderFromSame)
        { }
    }

    public class ProRataDistribution<T> : IEnumerable<ProRataShare<T>>
    {
        private readonly Dictionary<T, decimal> weights;
        private readonly int scale;
        private List<ProRataShare<T>> allocations;

        public decimal Total { get; private set; }

        /// <summary>
        /// True if the total was distributed with no remainder
        /// </summary>
        public bool IsDistributedEvenly { get { return Remainder == 0; } }

        /// <summary>
        /// The amount that could not be distributed by weight and was distributed according to 
        /// </summary>
        public decimal Remainder { get; private set; }

        public RemainderGoesTo RemainderGoesTo { get; private set; }

        public bool TakeNegativeRemainderFromSame { get; private set; }

        internal ProRataDistribution(decimal total, Dictionary<T, decimal> weights, int scale, RemainderGoesTo remainderGoesTo, bool takeNegativeRemainderFromSame)
        {
            if (weights == null)
                throw new ArgumentNullException("weights");

            if (weights.Any(w => w.Value == 0M))
                throw new ArgumentOutOfRangeException("weights", "Weights can not have a value of zero");

            this.Total = Money.SetScale(total, scale);
            this.weights = weights;
            this.scale = scale;
            this.RemainderGoesTo = remainderGoesTo;
            this.TakeNegativeRemainderFromSame = takeNegativeRemainderFromSame;

            PerformDistribution();
        }

        public IEnumerator<ProRataShare<T>> GetEnumerator()
        {
            return allocations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public decimal[] GetShares()
        {
            return this.Select(s => s.Share).ToArray();
        }

        private void PerformDistribution()
        {
            var totalWeight = weights.Sum(w => w.Value);

            // Initial allocations are calculated and temporarily sorted by Weight, Index ASC
            allocations = weights.Select((weight, i) => new ProRataShare<T>(weight.Key, i, weight.Value, Math.Round(Total * weight.Value / totalWeight, scale), Total)).OrderBy(s => s.Weight).ThenBy(s => s.Index).ToList();
            
            var missingUnits = (int)(Money.GetNumberOfSmallestAmountsInOne(scale) * (Total - allocations.Sum(a => a.Share)));
            var unit = Money.GetSmallestAmount(scale);

            Remainder = unit * missingUnits;

            if (!IsDistributedEvenly && RemainderGoesTo != RemainderGoesTo.DoNotDistribute)
            {
                var allocationCount = allocations.Count();

                if (missingUnits < 0)
                    unit = decimal.Negate(unit);

                missingUnits = Math.Abs(missingUnits);

                if (missingUnits > allocationCount)
                    throw new ApplicationException("More missing pennies than allocations");

                if (DistributeToSmallest(unit))
                {
                    for (int i = 0; i < missingUnits; i++)
                        allocations[i % allocationCount].Share += unit;
                }
                else if (DistributeToLargest(unit))
                {
                    for (int i = 0; i < missingUnits; i++)
                        allocations[allocationCount - (i % allocationCount) - 1].Share += unit;
                }
                else if (DistributeToSingleSmallest(unit))
                {
                    allocations[0].Share += unit * missingUnits;
                }
                else if (DistributeToSingleLargest(unit))
                {
                    allocations[allocationCount - 1].Share += unit * missingUnits;
                }
            }

            if (RemainderGoesTo == RemainderGoesTo.DoNotDistribute)
            {
                if (allocations.Sum(a => a.Share) + Remainder != Total)
                    throw new ApplicationException("An incorrect amount was allocated");
            }
            else
            {
                if (allocations.Sum(a => a.Share) != Total)
                    throw new ApplicationException("An incorrect amount was allocated");
            }

            // Put in the order that the weights were passed into this class
            allocations = allocations.OrderBy(s => s.Index).ToList();
        }

        private bool DistributeToSmallest(decimal unit)
        {
            return ((unit > 0 || TakeNegativeRemainderFromSame) && RemainderGoesTo == RemainderGoesTo.SmallestAmounts) ||
                (unit < 0 && !TakeNegativeRemainderFromSame && RemainderGoesTo == RemainderGoesTo.LargestAmounts);
        }

        private bool DistributeToLargest(decimal unit)
        {
            return ((unit > 0 || TakeNegativeRemainderFromSame) && RemainderGoesTo == RemainderGoesTo.LargestAmounts) ||
                (unit < 0 && !TakeNegativeRemainderFromSame && RemainderGoesTo == RemainderGoesTo.SmallestAmounts);
        }

        private bool DistributeToSingleSmallest(decimal unit)
        {
            return ((unit > 0 || TakeNegativeRemainderFromSame) && RemainderGoesTo == RemainderGoesTo.SingleSmallestAmount) ||
                (unit < 0 && !TakeNegativeRemainderFromSame && RemainderGoesTo == RemainderGoesTo.SingleLargestAmount);
        }

        private bool DistributeToSingleLargest(decimal unit)
        {
            return ((unit > 0 || TakeNegativeRemainderFromSame) && RemainderGoesTo == RemainderGoesTo.SingleLargestAmount) ||
                (unit < 0 && !TakeNegativeRemainderFromSame && RemainderGoesTo == RemainderGoesTo.SingleSmallestAmount);
        }
    }
}
