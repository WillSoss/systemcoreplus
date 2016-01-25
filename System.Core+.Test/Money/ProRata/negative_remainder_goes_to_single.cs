using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.CorePlus.Test.Money.ProRata
{
    public class negative_remainder_to_single_smallest_amount : prorata_distribution_test
    {
        public negative_remainder_to_single_smallest_amount()
            : base(.3M, RemainderGoesTo.SingleSmallestAmount, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(-.02M, .01M, .01M, 0M, 0M, 0M);
        }
    }

    public class negative_remainder_to_single_smallest_amount_take_from_opposite : prorata_distribution_test
    {
        public negative_remainder_to_single_smallest_amount_take_from_opposite()
            : base(.02M, RemainderGoesTo.SingleSmallestAmount, false, 1M, 1M, 1M)
        {
            Expect(.01M, .01M, .01M, 0M);
        }
    }    public class negative_remainder_to_single_largest_amount : prorata_distribution_test
    {
        public negative_remainder_to_single_largest_amount()
            : base(.03M, RemainderGoesTo.SingleLargestAmount, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(-.02M, .01M, .01M, 0M, .01M, 0M);
        }
    }

    public class negative_remainder_to_single_largest_amount_take_from_opposite : prorata_distribution_test
    {
        public negative_remainder_to_single_largest_amount_take_from_opposite()
            : base(.02M, RemainderGoesTo.SingleLargestAmount, false, 1M, 1M, 1M)
        {
            Expect(.01M, 0M, .01M, .01M);
        }
    }


}
