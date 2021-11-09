using System;
using System.Collections.Generic;
using Archi.Library.Filter;
using Archi.Library.Wrappers;

namespace Archi.Library.Helpers
{
    public class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedResponse<T>(List<T> pagedData, PaginationFilter validFilter, int totalRecords, IUriService uriService, string route)
        {
            var reponse = new PagedResponse<List<T>>(pagedData, validFilter.Range, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
            reponse.Next =
                validFilter.Range >= 1 && validFilter.Range < roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.Range + 1, validFilter.PageSize), route)
                : null;
            reponse.Prev =
                validFilter.Range - 1 >= 1 && validFilter.Range <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.Range - 1, validFilter.PageSize), route)
                : null;
            reponse.First = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
            reponse.Last = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
            reponse.TotalPages = roundedTotalPages;
            reponse.TotalRecords = totalRecords;
            return reponse;
        }
    }
}
