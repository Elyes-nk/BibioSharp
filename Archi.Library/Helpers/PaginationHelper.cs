using System;
using System.Collections.Generic;
using Archi.Library.Filter;
using Archi.Library.Wrappers;

namespace Archi.Library.Helpers
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, String range, PaginationFilter validFilter, int totalRecords, IUriService uriService, string route)
        {
            var reponse = new PagedResponse<List<T>>(pagedData, range, validFilter.Page, validFilter.PageSize);

            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            //TODO function for spliting range

            //split range
            var tab = range.Split('-');
            var start = int.Parse(tab[0]);
            var end = int.Parse(tab[1]);
            var pageSize = (end - start);

            //rel = "next"
            var nextStart = (1 + start + pageSize);
            var nextEnd = (1 + end + pageSize);
            if (nextEnd > totalRecords)
            {
                nextEnd = totalRecords;
            }
            string next = nextStart+ "-" +nextEnd;
            reponse.Next =
                nextStart >= 0 && nextStart < totalRecords
                ? uriService.GetPageUri(next, route)
                : null;

            //rel = "prev"
            var prevStart = (start - pageSize - 1);
            var prevEnd = (end - pageSize - 1);
            var prev = prevStart + "-" + prevEnd;
            reponse.Prev =
                prevStart - 1 >= 0 && prevEnd <= totalRecords
                ? uriService.GetPageUri(prev, route)
                : null;

            //rel = "first"
            var firstStart = 0;
            var firstEnd = (firstStart + pageSize);
            var first = firstStart + "-" + firstEnd;
            reponse.First = uriService.GetPageUri(first, route);

            //rel = "last"
            var lastStart = (totalRecords - pageSize);
            var last = lastStart + "-" + totalRecords;
            reponse.Last = uriService.GetPageUri(last, route);


            reponse.TotalPages = roundedTotalPages;
            reponse.TotalRecords = totalRecords;
            return reponse;
        }
    }
}
