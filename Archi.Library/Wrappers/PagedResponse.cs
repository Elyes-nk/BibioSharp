using System;
namespace Archi.Library.Wrappers
{

    public class PagedResponse<T> : Response<T>
    {
        public int Range { get; set; }
        public int PageSize { get; set; }
        public Uri First { get; set; }
        public Uri Last { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public Uri Next { get; set; }
        public Uri Prev { get; set; }
        public PagedResponse(T data, int range, int pageSize)
        {
            this.Range = range;
            this.PageSize = pageSize;
            this.Data = data;
            this.Message = null;
            this.Succeeded = true;
            this.Errors = null;
        }
    }
}

