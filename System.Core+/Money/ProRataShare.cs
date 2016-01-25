using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class ProRataShare
    {
        public int Index { get; private set; }
        public decimal Weight { get; private set; }
        public decimal Share { get; internal set; }
        public decimal Whole { get; private set; }

        public ProRataShare(int index, decimal weight, decimal share, decimal whole)
        {
            this.Index = index;
            this.Weight = weight;
            this.Share = share;
            this.Whole = whole;
        }
    }

    public class ProRataShare<T> : ProRataShare
    {
        public T Id { get; private set; }

        public ProRataShare(T id, int index, decimal weight, decimal share, decimal whole)
            : base(index, weight, share, whole)
        {
            this.Id = id;
        }
    }
}
