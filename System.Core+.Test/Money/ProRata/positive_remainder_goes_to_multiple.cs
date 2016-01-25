using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test.Money.ProRata
{
    public class positive_remainder_to_smallest_amounts : prorata_distribution_test
    {
        public positive_remainder_to_smallest_amounts()
            : base(.02M, RemainderGoesTo.SmallestAmounts, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(.02M, 0M, 0M, .01M, 0M, .01M);
        }
    }

    public class positive_remainder_to_largest_amounts : prorata_distribution_test
    {
        public positive_remainder_to_largest_amounts()
            : base(.02M, RemainderGoesTo.LargestAmounts, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(.02M, .01M, 0M, 0M, .01M, 0M);
        }
    }
}
