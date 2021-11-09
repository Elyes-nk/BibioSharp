using System;
namespace Archi.Library.Filter
{
    public class PaginationFilter
    {
        public int Range { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.Range = 1;
            this.PageSize = 7;
        }
        public PaginationFilter(int range, int pageSize)
        {
            this.Range = range < 1 ? 1 : range;
            this.PageSize = pageSize > 7 ? 7 : pageSize;
        }
    }
}
