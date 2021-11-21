using System;
using System.Collections.Generic;
using Archi.Library.Filter;
using Archi.Library.Wrappers;

namespace Archi.Library.Helpers
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, String range, PaginationFilter validFilter, int totalRecords, IUriService uriService, string route, string asc, string desc, string type, string rating, string date)
        {
            var reponse = new PagedResponse<List<T>>(pagedData, range, validFilter.Page, validFilter.PageSize);

            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            //TODO function for spliting range
            //split range
            var tab = range.Split('-');
            var start = int.Parse(tab[0]);
            var end = int.Parse(tab[1]);
            var validRange = new RangeFilter(start, end, totalRecords);
            var pageSize = (validRange.End - validRange.Start + 1);


            //rel = "first"
            var firstStart = 1;
            var firstEnd = (firstStart + pageSize - 1);
            var first = firstStart + "-" + firstEnd;
            reponse.First = uriService.GetPageUri(first, route, asc, desc, type, rating, date);


            //rel = "next"
            var nextStart = (validRange.Start + pageSize);
            var nextEnd = (validRange.End + pageSize);
            if (nextEnd > totalRecords)
            {
                nextEnd = totalRecords;
            }
            string next = nextStart+ "-" +nextEnd;
            reponse.Next =
                nextStart >= 1 && nextStart <= totalRecords
                ? uriService.GetPageUri(next, route, asc, desc, type, rating, date)
                : null;


            //rel = "prev"
            var prevStart = (validRange.Start - pageSize);
            var prevEnd = (validRange.End - pageSize);
            var prev = prevStart + "-" + prevEnd;
            reponse.Prev =
                prevStart - 1 >= 1 && prevEnd <= totalRecords
                ? uriService.GetPageUri(prev, route, asc, desc, type, rating, date)
                : null;


            //rel = "last"
            var lastStart = pageSize * (roundedTotalPages - 1) + 1;
            string last = lastStart + "-" + totalRecords;       
            reponse.Last = uriService.GetPageUri(last, route, asc, desc, type, rating, date);


            reponse.TotalPages = roundedTotalPages;
            reponse.TotalRecords = totalRecords;
            return reponse;
        }
    }
}
