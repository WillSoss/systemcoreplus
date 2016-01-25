using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// Determines how to deal with remainders in distributing money
    /// </summary>
    public enum RemainderGoesTo
    {
        /// <summary>
        /// The entire remainder goes to the single smallest amount.
        /// </summary>
        SingleSmallestAmount,

        /// <summary>
        /// The remainder is distributed in unit amounts starting with the smallest amount and working up.
        /// </summary>
        SmallestAmounts,

        /// <summary>
        /// The remainder is distributed in unit amounts starting with the largest amount and working down.
        /// </summary>
        LargestAmounts,

        /// <summary>
        /// The entire remainder goes to the single largest amount.
        /// </summary>
        SingleLargestAmount,

        /// <summary>
        /// The remainder is not distributed. The sum of the distributed amounts will be less than the total if the remainder is greater than zero.
        /// </summary>
        DoNotDistribute
    }
}
