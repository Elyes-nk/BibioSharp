using System;
using Microsoft.AspNetCore.WebUtilities;

namespace Archi.LibraryTests.Mock
{
    public class MockUriService : IUriService
    {
        private readonly string _baseUri;
        public MockUriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPageUri(string range, string route)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "range", range);
            return new Uri(modifiedUri);
        }
    }
}
