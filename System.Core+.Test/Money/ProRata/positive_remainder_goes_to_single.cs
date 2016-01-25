using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace System.CorePlus.Test.Money.ProRata
{
    public class remainder_to_single_smallest_amount : prorata_distribution_test
    {
        public remainder_to_single_smallest_amount()
             : base(.02M, RemainderGoesTo.SingleSmallestAmount, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(.02M, 0M, 0M, 0M, 0M, .02M);
        }
    }

    public class remainder_to_single_largest_amount : prorata_distribution_test
    {
        public remainder_to_single_largest_amount()
            : base(.02M, RemainderGoesTo.SingleLargestAmount, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(.02M, 0M, 0M, .0M, .02M, 0M);
        }
    }
}
