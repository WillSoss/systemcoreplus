using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.CorePlus.Test.Money.ProRata
{
    public class positive_remainder_not_distributed : prorata_distribution_test
    {
        public positive_remainder_not_distributed()
            : base(.02M, RemainderGoesTo.DoNotDistribute, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(.02M, 0M, 0M, 0M, 0M, 0M);
        }
    }

    public class negative_remainder_not_distributed : prorata_distribution_test
    {
        public negative_remainder_not_distributed()
            : base(.03M, RemainderGoesTo.DoNotDistribute, true, 1.1M, 1M, .9M, 1.15M, .85M)
        {
            Expect(-.02M, .01M, .01M, .01M, .01M, .01M);
        }
    }
}
