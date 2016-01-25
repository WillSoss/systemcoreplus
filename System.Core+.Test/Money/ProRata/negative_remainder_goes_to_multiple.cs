using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.CorePlus.Test.Money.ProRata
{
    public class negative_remainder_to_smallest_amounts : prorata_distribution_test
    {
        public negative_remainder_to_smallest_amounts()
            : base(.03M, RemainderGoesTo.SmallestAmounts, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(-.02M, .01M, .01M, 0M, .01M, 0M);
        }
    }

    public class negative_remainder_to_smallest_amounts_take_from_opposite : prorata_distribution_test
    {
        public negative_remainder_to_smallest_amounts_take_from_opposite()
            : base(.03M, RemainderGoesTo.SmallestAmounts, false, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(-.02M, 0M, .01M, .01M, 0M, .01M);
        }
    }

    public class negative_remainder_to_largest_amounts : prorata_distribution_test
    {
        public negative_remainder_to_largest_amounts()
            : base(.03M, RemainderGoesTo.LargestAmounts, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(-.02M, 0M, .01M, .01M, 0M, .01M);
        }
    }

    public class negative_remainder_to_largest_amounts_take_from_opposite : prorata_distribution_test
    {
        public negative_remainder_to_largest_amounts_take_from_opposite()
            : base(.03M, RemainderGoesTo.LargestAmounts, false, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(-.02M, .01M, .01M, 0M, .01M, 0M);
        }
    }
}
