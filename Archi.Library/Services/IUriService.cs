using System;
using Archi.Library.Filter;


public interface IUriService
{
    public Uri GetPageUri(String range, string route, string asc, string desc, string type, string rating, string date);
}