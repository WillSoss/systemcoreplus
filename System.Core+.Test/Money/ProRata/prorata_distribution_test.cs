using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test.Money.ProRata
{
    [Trait("Category", "ProRata")]
    public abstract class prorata_distribution_test : Specification
    {
        private decimal whole;
        private RemainderGoesTo remainderGoesTo;
        private bool takeNegativeRemainderFromSame;
        private decimal[] weights;


        private decimal expectedRemainder;
        private decimal[] expectedShares;

        protected System.ProRataDistribution dist;

        protected prorata_distribution_test(decimal whole, RemainderGoesTo remainderGoesTo, bool takeNegativeRemainderFromSame, params decimal[] weights)
        {
            this.whole = whole;
            this.remainderGoesTo = remainderGoesTo;
            this.takeNegativeRemainderFromSame = takeNegativeRemainderFromSame;
            this.weights = weights;
        }

        protected prorata_distribution_test(decimal whole, params decimal[] weights)
        {
            this.whole = whole;
            this.remainderGoesTo = RemainderGoesTo.SmallestAmounts;
            this.takeNegativeRemainderFromSame = true;
            this.weights = weights;
        }

        protected void Expect(decimal remainder, params decimal[] shares)
        {
            if (shares.Length != weights.Length)
                throw new ArgumentException("There must be an equal number of expected shares as there are weights");

            this.expectedShares = shares;
            this.expectedRemainder = remainder;
        }

        protected override void Observe()
        {
            dist = System.Money.DistributeProRata(whole, weights, remainderGoesTo: remainderGoesTo, takeNegativeRemainderFromSame: takeNegativeRemainderFromSame);
        }

        [Observation]
        public void should_set_total()
        {
            dist.Total.ShouldEqual(whole);
        }

        [Observation]
        public void should_set_remainder()
        {
            if (expectedShares != null)
            {
                dist.Remainder.ShouldEqual(expectedRemainder);
                dist.IsDistributedEvenly.ShouldEqual(expectedRemainder == 0M);
            }
        }

        [Observation]
        public void should_distrbute_expected_shares()
        {
            if (expectedShares != null)
            {
                var shares = dist.GetShares();
                for (int i = 0; i < expectedShares.Length; i++)
                    shares[i].ShouldEqual(expectedShares[i]);   
            }
        }
    }
}
