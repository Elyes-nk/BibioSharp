using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Archi.Library.Models;

namespace APILibrary.Core.Extensions
{
    public static class IQueryableExtensions
    {
        public enum OperationExpression
        {
            Equals,
            NotEquals,
            Minor,
            MinorEquals,
            Mayor,
            MayorEquals,
            Like,
            Contains,
            Any
        }




        //----------------------------------------------------------recherche------------------------------------------------------------------------------------------------------------------------

        public static IQueryable<TModel> SearchThis<TModel>(this IQueryable<TModel> contents, string search ) where TModel : ModelBase
        {
            Regex propNameAndsearchName = new(@"\b\=\b");

            IQueryable<TModel> contentSearched = null;
            if (!string.IsNullOrEmpty(search))
            {
                //extraction de toutes les recherches
                var tabs = search.Split('&');
                foreach(var tab in tabs)
                {
                    //si la recherche est sous la forme nom de l'attribut = valeur 
                    if (propNameAndsearchName.IsMatch(tab))
                    {
                        //extraire le nom de l'attribut et ça valeur
                        string[] types = search.Split('=');
                        string propName = types[0];
                        string searchName = types[1];

                        //création des predicats de l'attribut égale à ça valeur 
                        var predicate = GetCriteriaWhere<TModel>(propName, OperationExpression.Contains, searchName);
                        //recherche du prédicat et affectation dans une variable
                        var contentsSearch = contents.Where(predicate);
                        if (contentSearched == null)
                        {
                            contentSearched = contentsSearch;
                        }
                        else
                        {
                            contentSearched = contentSearched.Concat(contentsSearch);
                        }
                        //affection de toute les recherche dans contents 
                        contents = contents.Intersect(contentSearched);
                    }
                }
            }
            return contents;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------








        //--------------------------------------------------------------------tris-------------------------------------------------------------------------------------------------------------
        public static IQueryable<TModel> OrderThis<TModel>(this IQueryable<TModel> contents, string asc, string desc) where TModel : ModelBase
        {
            if (string.IsNullOrEmpty(desc))
            {
                //affectation de la proprieté 
                var propInfo = typeof(TModel).GetProperty(asc, BindingFlags.Public |BindingFlags.IgnoreCase | BindingFlags.Instance);
                //si la proprieté que l'utilisateur à entrer n'existe pas
                if (propInfo is null)
                    throw new InvalidOperationException("Please provide a valid property name");
                //sinon faire un orderBY la propriété que l'utilisateur à entrer 
                else
                {
                    var keySelector = GetExpression<TModel>(propInfo.Name);
                    contents = contents.OrderBy(keySelector);
                }
            }
            else if (string.IsNullOrEmpty(asc))
            {
                var propInfo = typeof(TModel).GetProperty(desc, BindingFlags.Public |BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propInfo is null)
                    throw new InvalidOperationException("Please provide a valid property name");
                else
                {
                    var keySelector = GetExpression<TModel>(propInfo.Name);
                    contents = contents.OrderByDescending(keySelector);
                }
            }
            else
            {
                var propInfoAsc = typeof(TModel).GetProperty(asc, BindingFlags.Public |BindingFlags.IgnoreCase | BindingFlags.Instance);
                var propInfoDesc = typeof(TModel).GetProperty(desc, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propInfoAsc is null || propInfoDesc is null)
                    throw new InvalidOperationException("Please provide a valid property name");
                else
                {
                    var keySelector1 = GetExpression<TModel>(propInfoAsc.Name);
                    var keySelector2 = GetExpression<TModel>(propInfoDesc.Name);
                    contents = contents.OrderBy(keySelector1).ThenByDescending(keySelector2);
                }
            }
            return contents;
        }
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------











//--------------------------------------------------------------------filter-----------------------------------------------------------------------------------------------------------
        public static IQueryable<TModel> FilterThis<TModel>(this IQueryable<TModel> contents, string type, string rating, string date) where TModel : ModelBase
        {
            Regex typeAndType = new(@"\b\,\b");

            Regex ratingAndRating = new(@"\b\d\,\d\b");
            Regex ratingToRating = new(@"\[\b\d\,\d\b\]");
            Regex ratingToEnd = new(@"\[\d\b\,\]");
            Regex startToRating = new(@"\[\,\d\b\]");

            Regex dateAndDate = new(@"\b\d{0,4}\-\d{0,2}\-\d{0,2}\,\d{0,4}\-\d{0,2}\-\d{0,2}\b");
            Regex dateToDate = new(@"\[\b\d{0,4}\-\d{0,2}\-\d{0,2}\,\d{0,4}\-\d{0,2}\-\d{0,2}\b\]");
            Regex dateToEnd = new(@"\[\b\d{0,4}\-\d{0,2}\-\d{0,2}\b\,\]");
            Regex startToDate = new(@"\[\,\b\d{0,4}\-\d{0,2}\-\d{0,2}\b\]");

            if (!string.IsNullOrEmpty(type))
            {

                var propInfo = typeof(TModel).GetProperty("Type", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                var fieldName = propInfo.Name;

                //type,type recherche de deux types en même temps
                if (typeAndType.IsMatch(type))
                {
                    string[] types = type.Split(',');

                    //extraction des deux types
                    string one = types[0];
                    string two = types[1];

                    //création des predicats de ratings égale au type one, et égale au type two
                    var predicateOne = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, one);
                    var predicateTwo = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, two);

                    //recherche des produits grace au prédicats
                    var contentsOne = contents.Where(predicateOne);
                    var contentsTwo = contents.Where(predicateTwo);

                    //concationation des deux recherches
                    contents = contentsOne.Concat(contentsTwo);
                }
                //type recherche d'un seul type
                else
                {
                    var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, type);
                    contents = contents.Where(predicate);
                }
            }

            if (!string.IsNullOrEmpty(rating))
                {
                    var propInfo = typeof(TModel).GetProperty("Rating", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    var fieldName = propInfo.Name;
                    // [rating,rating] 
                    if (ratingToRating.IsMatch(rating))
                    {
                        //extraction des deux ratings
                        var start = rating[1].ToString();
                        var end = rating[rating.Length - 2].ToString();

                        //création des predicats de ratings supérieur au rating start, et inferieur au rating end
                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToInt32(start));
                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToInt32(end));

                        //recherche des predicats
                        var contentsStart = contents.Where(predicateStart);
                        var contentsEnd = contents.Where(predicateEnd);

                        //intersection pour avoir les rating superieur à start et inferieur à end
                        contents = contentsStart.Intersect(contentsEnd);
                    }
                    // rating,rating 
                    else if (ratingAndRating.IsMatch(rating))
                    {
                        //extraction des deux ratings
                        string[] ratings = rating.Split(',');

                        //création des predicats de ratings égale au rating start, et égale au rating end
                        string one = ratings[0];
                        string two = ratings[1];

                        //création des predicats de ratings égale au rating start, et égale au rating end
                        var predicateOne = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToInt32(one));
                        var predicateTwo = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToInt32(two));

                        //recherche des produits
                        var contentsOne = contents.Where(predicateOne);
                        var contentsTwo = contents.Where(predicateTwo);

                        //concatination des deux produits
                        contents = contentsOne.Concat(contentsTwo);

                    }
                    // [rating,] 
                    else if (ratingToEnd.IsMatch(rating))
                    {
                        var start = rating[1].ToString();
                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToInt32(start));
                        contents = contents.Where(predicateStart);
                    }
                    // [,rating] 
                    else if (startToRating.IsMatch(rating))
                    {
                        var end = rating[rating.Length - 2].ToString();
                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToInt32(end));
                        contents = contents.Where(predicateEnd);
                    }
                    //rating
                    else
                    {
                        var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToInt32(rating));
                        contents = contents.Where(predicate);
                    }
            }

            if (!string.IsNullOrEmpty(date))
            {
                    var propInfo = typeof(TModel).GetProperty("Date", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    var fieldName = propInfo.Name;

                    // [date,date] 
                    if (dateToDate.IsMatch(date))
                    {
                        var start = date.Substring(1, 10);
                        var end = date.Substring(12, 10);

                        string dateTimeStart = start + "T00:00:00";
                        DateTime dtStart = DateTime.ParseExact(dateTimeStart, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                        string dateTimeEnd = end + "T00:00:00";
                        DateTime dtEnd = DateTime.ParseExact(dateTimeEnd, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, dtStart);
                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, dtEnd);

                        var contentsOne = contents.Where(predicateStart);
                        var contentsTwo = contents.Where(predicateEnd);
                        contents = contentsOne.Intersect(contentsTwo);
                    }
                    //date,date
                    else if (dateAndDate.IsMatch(date))
                    {
                        string[] dates = date.Split(',');
                        string one = dates[0].Substring(0, 10);
                        string two = dates[1].Substring(0,10);

                        string dateTimeOne = one + "T00:00:00";
                        DateTime dtOne = DateTime.ParseExact(dateTimeOne, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                        string dateTimeTwo = two + "T00:00:00";
                        DateTime dtTwo = DateTime.ParseExact(dateTimeTwo, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                        var predicateOne = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, dtOne);
                        var predicateTwo = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, dtTwo);

                        var contentsOne = contents.Where(predicateOne);
                        var contentsTwo = contents.Where(predicateTwo);
                        contents = contentsOne.Concat(contentsTwo);
                    }
                    //[date,]
                    else if (dateToEnd.IsMatch(date))
                    {
                        var start = date.Substring(1, 10);

                        string dateTimeStart = start + "T00:00:00";
                        DateTime dtStart = DateTime.ParseExact(dateTimeStart, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, dtStart);
                        contents = contents.Where(predicateStart);
                    }
                    //[,date]
                    else if (startToDate.IsMatch(date))
                    {
                        var end = date.Substring(2, 10);

                        string dateTimeEnd = end + "T00:00:00";
                        DateTime dtEnd = DateTime.ParseExact(dateTimeEnd, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, dtEnd);
                        contents = contents.Where(predicateEnd);
                    }
                    //date
                    else
                    {
                        string dateTime = date+"T00:00:00";
                        DateTime dt = DateTime.ParseExact(dateTime, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                        var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, dt);
                        contents = contents.Where(predicate);
                    }
            }
            return contents;
        }
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------






        //FONCTION générique qui permet la recherche par apport au nom de l'attribut et ça valeur
        //ainsi que l'operateur logique correspondant qui peut être (supérieur, inférieur, égale.. etc)
        public static Expression<Func<TModel, bool>> GetCriteriaWhere<TModel>(string fieldName, OperationExpression selectedOperator, object fieldValue) where TModel : ModelBase
        {

            var propInfo = typeof(TModel).GetProperty(fieldName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);

            var parameter = Expression.Parameter(typeof(TModel), "x");
            var expressionParameter = GetMemberExpression<TModel>(parameter, fieldName);
            if (propInfo != null && fieldValue != null)
            {

                BinaryExpression body = null;

                switch (selectedOperator)
                {
                    case OperationExpression.Equals:
                        body = Expression.Equal(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.NotEquals:
                        body = Expression.NotEqual(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.Minor:
                        body = Expression.LessThan(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.MinorEquals:
                        body = Expression.LessThanOrEqual(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.Mayor:
                        body = Expression.GreaterThan(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.MayorEquals:
                        body = Expression.GreaterThanOrEqual(expressionParameter, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(body, parameter);
                    case OperationExpression.Like:
                        MethodInfo contains = typeof(string).GetMethod("Contains");
                        var bodyLike = Expression.Call(expressionParameter, contains, Expression.Constant(fieldValue, propInfo.PropertyType));
                        return Expression.Lambda<Func<TModel, bool>>(bodyLike, parameter);
                    case OperationExpression.Contains:
                        return Contains<TModel>(fieldName, fieldValue, parameter, expressionParameter);
                    default:
                        throw new Exception("Not implement Operation");
                }
            }
            else
            {
                Expression<Func<TModel, bool>> filter = x => true;
                return filter;
            }
        }

        private static MemberExpression GetMemberExpression<TModel>(ParameterExpression parameter, string propName) where TModel : ModelBase
        {
            if (string.IsNullOrEmpty(propName)) return null;
            else
            {
                return Expression.Property(parameter, propName);
            }
        }

        public static Expression<Func<TModel, object>> GetExpression<TModel>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TModel), "x");
            Expression conversion = Expression.Convert(Expression.Property(param, propertyName), typeof(object));
            return Expression.Lambda<Func<TModel, object>>(conversion, param);
        }

        private static Expression<Func<TModel, bool>> Contains<TModel>(string fieldName, object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression) where TModel : ModelBase
        {
            var propertyExp = Expression.Property(parameterExpression, fieldName);
            if (propertyExp.Type == typeof(string))
            {
                MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var someValue = Expression.Constant(fieldValue, typeof(string));
                var containsMethodExp = Expression.Call(propertyExp, method, someValue);
                return Expression.Lambda<Func<TModel, bool>>(containsMethodExp, parameterExpression);
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(propertyExp.Type);
                var result = converter.ConvertFrom(fieldValue);
                var someValue = Expression.Constant(result);
                var containsMethodExp = Expression.Equal(propertyExp, someValue);
                return Expression.Lambda<Func<TModel, bool>>(containsMethodExp, parameterExpression);
            }
        }
    }

    ////SearchByName(IQueyable<TModel> query, string name)
    //public static IQueryable<TModel> SearchByName<TModel>(this IQueryable<TModel> query, string name) where TModel : ModelBase
    //{
    //    var propsInfo = typeof(TModel).GetProperties();
    //    var result = query;
    //    foreach (var prop in propsInfo)
    //    {
    //        if (prop.PropertyType == typeof(string))
    //        {
    //            var predicate = GetCriteriaWhere<TModel>(prop.Name, OperationExpression.Contains, name);
    //            if (query.Where(predicate).Count() > 0)
    //            {
    //                result = result.Where(predicate);
    //            }
    //        }
    //    }
    //    return result;
    //}
}
