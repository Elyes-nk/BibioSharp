using System;
namespace Archi.Library.Filter
{
    public class RangeFilter
    {
        public int Start { get; set; }
        public int End { get; set; }

        public RangeFilter(int start, int end, int limit)
        {
            this.End = start == 0 ? end + 1 : end;
            this.Start = start == 0 ? start + 1 : start;
            this.End = end > limit ? limit : end;

        }
    }
}
