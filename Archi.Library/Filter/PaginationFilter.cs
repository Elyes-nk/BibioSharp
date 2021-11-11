using System;
namespace Archi.Library.Filter
{
    public class PaginationFilter
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PaginationFilter()
        {
            this.Page = 0;
            this.PageSize = 7;
        }
        public PaginationFilter(int page, int pageSize)
        {
            this.Page = page < 0 ? 0 : page;
            this.PageSize = pageSize > 50 ? 50 : pageSize;
        }
    }
}
