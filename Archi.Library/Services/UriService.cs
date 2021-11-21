using System;
using Archi.Library.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Archi.Library.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPageUri(string range, string route, string asc, string desc, string type, string rating, string date)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));


            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "range", range);
            if (!string.IsNullOrEmpty(asc))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "asc", asc);
            }
            if (!string.IsNullOrEmpty(desc))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "desc", desc);
            }
            if (!string.IsNullOrEmpty(type))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "type", type);
            }
            if (!string.IsNullOrEmpty(rating))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "rating", rating);
            }
            if (!string.IsNullOrEmpty(date))
            {
                modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "date", date);
            }


            return new Uri(modifiedUri);
        }

    }
}
