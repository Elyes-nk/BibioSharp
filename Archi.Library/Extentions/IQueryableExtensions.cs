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

//--------------------------------------------------------------------tris-------------------------------------------------------------------------------------------------------------
        public static IQueryable<TModel> OrderThis<TModel>(this IQueryable<TModel> contents, string asc, string desc) where TModel : ModelBase
        {
            if (string.IsNullOrEmpty(desc))
            {
                var propInfo = typeof(TModel).GetProperty(asc, BindingFlags.Public |
                    BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (propInfo is null)
                    throw new InvalidOperationException("Please provide a valid property name");
                else
                {
                    var keySelector = GetExpression<TModel>(propInfo.Name);
                    contents = contents.OrderBy(keySelector);
                }
            }
            else if (string.IsNullOrEmpty(asc))
            {
                var propInfo = typeof(TModel).GetProperty(desc, BindingFlags.Public |
                   BindingFlags.IgnoreCase | BindingFlags.Instance);
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
                var propInfoAsc = typeof(TModel).GetProperty(asc, BindingFlags.Public |
                  BindingFlags.IgnoreCase | BindingFlags.Instance);
                var propInfoDesc = typeof(TModel).GetProperty(desc, BindingFlags.Public |
                  BindingFlags.IgnoreCase | BindingFlags.Instance);
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
            Regex ratingToRating = new(@"\[\d*\,\d*\]");
            Regex ratingToEnd = new(@"\[\d*\,\]");
            Regex startToRating = new(@"\[\,\d*\]");
            Regex dateAndDate = new(@"\b\d{0,4}\-\d{0,2}\-\d{0,2}\,\d{0,4}\-\d{0,2}\-\d{0,2}\b");
            Regex dateToDate = new(@"\[\b\d{0,4}\-\d{0,2}\-\d{0,2}\,\d{0,4}\-\d{0,2}\-\d{0,2}\b\]");
            Regex dateToEnd = new(@"\[\b\d{0,4}\-\d{0,2}\-\d{0,2}\b\,\]");
            Regex startToDate = new(@"\[\,\b\d{0,4}\-\d{0,2}\-\d{0,2}\b\]");



     
                if (!string.IsNullOrEmpty(type))
                {
                    var propInfo = typeof(TModel).GetProperty("Type", BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    var fieldName = propInfo.Name;
                    if (typeAndType.IsMatch(rating))
                    {

                    }
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
                    if (ratingToRating.IsMatch(rating))
                    {
                        var start = rating[1].ToString();
                        var end = rating[rating.Length - 2].ToString();
                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToDecimal(start));
                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToDecimal(end));
                        contents = contents.Where(predicateStart).Where(predicateEnd);
                    }
                    else if (ratingAndRating.IsMatch(rating))
                    {
                        
                    }
                    else if (ratingToEnd.IsMatch(rating))
                    {
                        var start = rating[1].ToString();
                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToDecimal(start));
                        contents = contents.Where(predicateStart);
                    }
                    else if (startToRating.IsMatch(rating))
                    {
                        var end = rating[rating.Length - 2].ToString();
                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToDecimal(end));
                        contents = contents.Where(predicateEnd);
                    }
                    else
                    {
                        var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToDecimal(rating));
                        contents = contents.Where(predicate);
                    }
                }





                if (!string.IsNullOrEmpty(date))
                {
                    var propInfo = typeof(TModel).GetProperty("CreateAt", BindingFlags.Public |
                   BindingFlags.IgnoreCase | BindingFlags.Instance);
                    var fieldName = propInfo.Name;
                    if (dateToDate.IsMatch(date))
                    {
                        var start = date.Substring(1, 10);
                        var end = date.Substring(12, 10);
                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToDateTime(start));
                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToDateTime(end));
                        contents = contents.Where(predicateStart).Where(predicateEnd);
                    }
                    else if (dateAndDate.IsMatch(date))
                    {

                    }
                    else if (dateToEnd.IsMatch(date))
                    {
                        var start = date.Substring(1, 10);
                        var predicateStart = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MayorEquals, Convert.ToDateTime(start));
                        contents = contents.Where(predicateStart);
                    }
                    else if (startToDate.IsMatch(date))
                    {
                        var end = date.Substring(2, 10);
                        var predicateEnd = GetCriteriaWhere<TModel>(fieldName, OperationExpression.MinorEquals, Convert.ToDateTime(end));
                        contents = contents.Where(predicateEnd);
                    }
                    else
                    {
                        var predicate = GetCriteriaWhere<TModel>(fieldName, OperationExpression.Equals, Convert.ToDateTime(date));
                        contents = contents.Where(predicate);
                    }
                }
            return contents;
        }
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------







        //function filter
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





        //function function filter
        private static MemberExpression GetMemberExpression<TModel>(ParameterExpression parameter, string propName) where TModel : ModelBase
        {
            if (string.IsNullOrEmpty(propName)) return null;
            else
            {
                return Expression.Property(parameter, propName);
            }
            /*var propertiesName = propName.Split('.');
            if (propertiesName.Count() == 2)
                return Expression.Property(Expression.Property(parameter, propertiesName[0]), propertiesName[1]);*/
        }






        public static Expression<Func<TModel, object>> GetExpression<TModel>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TModel), "x");
            Expression conversion = Expression.Convert(Expression.Property(param, propertyName), typeof(object));   //important to use the Expression.Convert
            return Expression.Lambda<Func<TModel, object>>(conversion, param);
        }





        //à modifier
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
}
