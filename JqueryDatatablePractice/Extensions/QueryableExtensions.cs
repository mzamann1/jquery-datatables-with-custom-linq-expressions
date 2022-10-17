using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JqueryDatatablePractice.Constants;
using JqueryDatatablePractice.Models.ViewModels;

namespace JqueryDatatablePractice.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string key, int value)
        {
            try
            {
                var type = typeof(TSource);
                var pe = Expression.Parameter(type);

                var propertyReference = Expression.Property(pe, key);
                var constantReference = Expression.Constant(value);
                var binaryExpression = Expression.Equal(propertyReference, constantReference);
                Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(binaryExpression, pe);
                return source.Where(lambda);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return source;
            }

        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string key, string value, MethodType methodType = MethodType.Equal)
        {
            /*
            *   Check if Key is null or empty space, or
            *   if someone has provided only the key 
            */
            if (string.IsNullOrWhiteSpace(key) || (string.IsNullOrWhiteSpace(value) && methodType != MethodType.Empty))
            {
                return source;
            }

            /*
            * Checking the type of TSource Object , for further processing
            */

            Type sourceType = typeof(TSource);

            /*
            * Creating Parameter Expression  t = >  , parameter  t will be of type TSource
            */

            var parameterExp = Expression.Parameter(sourceType, "t");

            /*
            * Getting Type of the Key tName
            */

            var propertyType = GetPropertyType(sourceType, key);

            /*
            * Now we need to create expression for that property
            */

            var memberExp = sourceType.GetProperty(key) == null ? default : Expression.Property(parameterExp, key);

            /*
            * Now we need to convert the provided value's datatype to the property type from which it is going to be matched.
            */

            var convertedConstvalue = Expression.Convert(Expression.Constant(value), propertyType);

            Expression finalExp = default;

            switch (methodType)
            {

                case MethodType.Empty:
                    finalExp = Expression.Call(typeof(string), nameof(string.IsNullOrWhiteSpace), null, memberExp);
                    break;

                case MethodType.StartsWith:

                    /*
                        * Getting StartsWith Method from string class using reflection
                    */

                    var startsWithInfo = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
                    finalExp = Expression.Call(memberExp, startsWithInfo, convertedConstvalue);
                    break;

                case MethodType.EndsWith:

                    /*
                         * Getting EndsWith Method from string class using reflection
                    */

                    var endsWithInfo = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
                    finalExp = Expression.Call(memberExp, endsWithInfo, convertedConstvalue);
                    break;

                case MethodType.Contains:

                    /*
                         * Getting EndsWith Method from string class using reflection
                    */

                    var containsInfo = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
                    finalExp = Expression.Call(memberExp, containsInfo, convertedConstvalue);
                    break;

                case MethodType.Equal:

                    finalExp = Expression.Equal(memberExp, convertedConstvalue);
                    break;

                case MethodType.NotEqual:

                    finalExp = Expression.NotEqual(memberExp, convertedConstvalue);
                    break;

                default:
                    break;
            }

            var lambda = Expression.Lambda(finalExp, false, parameterExp);
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

            return source.Provider.CreateQuery<TSource>(whereExpression);
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string key, object value, ConditionalOperatorType operatorType = ConditionalOperatorType.Empty)
        {
            /*
            *   Check if Key is null or empty space, or
            *   if someone has provided only the key 
            */

            if (string.IsNullOrWhiteSpace(key))
            {
                return source;
            }

            /*
            * Checking the type of TSource Object , for further processing
            */

            Type sourceType = typeof(TSource);

            /*
            * Creating Parameter Expression  t = >  , parameter  t will be of type TSource
            */

            var parameterExp = Expression.Parameter(sourceType, "t");

            /*
            * Getting Type of the Key tName
            */

            var propertyType = GetPropertyType(sourceType, key);

            /*
            * Now we need to create expression for that property
            */

            var memberExp = sourceType.GetProperty(key) == null ? default : Expression.Property(parameterExp, key);

            /*
            * Now we need to convert the provided value's datatype to the property type from which it is going to be matched.
            */


            if (!IsNumericType(memberExp.Type) || !IsNumericValue(value))
            {
                return source;
            }

            var convertedConstvalue = Expression.Convert(Expression.Constant(value), propertyType);

            Expression finalExp = default;

            switch (operatorType)
            {

                case ConditionalOperatorType.Empty:
                    return source;

                case ConditionalOperatorType.Equals:

                    finalExp = Expression.Equal(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.NotEquals:

                    finalExp = Expression.NotEqual(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.GreaterThan:

                    finalExp = Expression.GreaterThan(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.GreaterThanOrEqual:

                    finalExp = Expression.GreaterThanOrEqual(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.LessThan:

                    finalExp = Expression.LessThan(memberExp, convertedConstvalue);
                    break;

                case ConditionalOperatorType.LessThanOrEqual:

                    finalExp = Expression.LessThanOrEqual(memberExp, convertedConstvalue);
                    break;

                default:
                    break;
            }

            var lambda = Expression.Lambda(finalExp, false, parameterExp);
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

            return source.Provider.CreateQuery<TSource>(whereExpression);
        }


        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, IEnumerable<DtColumn> searachableCols, string value, MethodType methodType = MethodType.Equal)
        {
            if (string.IsNullOrEmpty(value) && methodType != MethodType.Empty)
            {
                return source;
            }

            var containsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
            var endsWithMethodInfo = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
            var toLowerMethodInfo = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
            var startsWithMethodInfo = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
            var trimMethodInfo = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes);

            Type entityType = typeof(TSource);
            var parameterExp = Expression.Parameter(entityType, "x");
            var expressions = new List<Expression>();


            var properties = entityType.GetProperties().Join(searachableCols, prop1 => prop1.Name, prop2 => prop2.Name, (prop1, prop2) => new
            {
                prop1.PropertyType,
                prop1.Name
            });


            foreach (var property in properties)
            {
                if (!property.PropertyType.Equals(typeof(string)))
                {
                    continue;
                }

                var propertyExp = Expression.PropertyOrField(parameterExp, property.Name);
                var constValueExp = Expression.Convert(Expression.Constant(value), property.PropertyType);
                switch (methodType)
                {
                    case MethodType.Empty:
                        // Translate to Expression
                        // x.Property == null || x.Property.Trim() == string.Empty
                        var constNullExp = Expression.Convert(Expression.Constant(null), property.PropertyType);
                        var constEmptyExp = Expression.Convert(Expression.Constant(string.Empty), property.PropertyType);
                        var equalToNullExp = Expression.Equal(propertyExp, constNullExp);
                        var equalToEmptyExp = Expression.Equal(Expression.Call(propertyExp, trimMethodInfo), constEmptyExp);
                        var blankExp = Expression.OrElse(equalToNullExp, equalToEmptyExp);

                        // string.IsNullOrWhiteSpace(x.Property)
                        // Performance wise, this method is faster..
                        var isNullOrWhiteSpaceExp = Expression.Call(typeof(string), nameof(string.IsNullOrWhiteSpace), null, propertyExp);
                        expressions.Add(isNullOrWhiteSpaceExp);
                        break;
                    case MethodType.Equal:
                        var equalExp = Expression.Equal(propertyExp, constValueExp);
                        expressions.Add(equalExp);
                        break;
                    case MethodType.NotEqual:
                        var notEqualExp = Expression.NotEqual(propertyExp, constValueExp);
                        expressions.Add(notEqualExp);
                        break;
                    case MethodType.Contains:
                        var containsExp = Expression.Call(propertyExp, containsMethodInfo, constValueExp);
                        expressions.Add(containsExp);
                        break;
                    case MethodType.StartsWith:
                        var startsWithExp = Expression.Call(propertyExp, startsWithMethodInfo, constValueExp);
                        expressions.Add(startsWithExp);
                        break;
                    case MethodType.EndsWith:
                        var endsWithExp = Expression.Call(propertyExp, endsWithMethodInfo, constValueExp);
                        expressions.Add(endsWithExp);
                        break;
                    default:
                        break;
                }
            }

            if (expressions.Count == 0)
            {
                return source;
            }

            var resultExp = expressions.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda(resultExp, false, parameterExp);
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { entityType }, source.Expression, lambda);
            return source.Provider.CreateQuery<TSource>(whereExpression);
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, IEnumerable<DtColumn> searachableCols, object value, ConditionalOperatorType condition = ConditionalOperatorType.Equals)
        {
            if ((value == null || !IsNumericValue(value) && condition != ConditionalOperatorType.Empty))
            {
                return source;
            }

            Type entityType = typeof(TSource);
            var parameterExp = Expression.Parameter(entityType, "x");
            var expressions = new List<Expression>();

            var properties = entityType.GetProperties().Join(searachableCols, prop1 => prop1.Name, prop2 => prop2.Name, (prop1, prop2) => new
            {
                prop1.PropertyType,
                prop1.Name
            });
            foreach (var property in properties)
            {
                if (!IsNumericType(property.PropertyType))
                {
                    continue;
                }

                var propertyExp = Expression.PropertyOrField(parameterExp, property.Name);
                var constValueExp = Expression.Convert(Expression.Constant(GetMaxValueIfOutOfRange(value, property.PropertyType)), property.PropertyType);
                switch (condition)
                {
                    case ConditionalOperatorType.Empty:
                        if (Nullable.GetUnderlyingType(property.PropertyType) == null)
                        {
                            // Property is not nullable ..
                            continue;
                        }

                        var equalToNullExp = Expression.Equal(propertyExp, Expression.Convert(Expression.Constant(null), property.PropertyType));
                        expressions.Add(equalToNullExp);
                        break;
                    case ConditionalOperatorType.Equals:
                        var equalExp = Expression.Equal(propertyExp, constValueExp);
                        expressions.Add(equalExp);
                        break;
                    case ConditionalOperatorType.NotEquals:
                        var notEqualExp = Expression.NotEqual(propertyExp, constValueExp);
                        expressions.Add(notEqualExp);
                        break;
                    case ConditionalOperatorType.GreaterThan:
                        var greaterExp = Expression.GreaterThan(propertyExp, constValueExp);
                        expressions.Add(greaterExp);
                        break;
                    case ConditionalOperatorType.GreaterThanOrEqual:
                        var greaterOrEqualExp = Expression.GreaterThanOrEqual(propertyExp, constValueExp);
                        expressions.Add(greaterOrEqualExp);
                        break;
                    case ConditionalOperatorType.LessThan:
                        var lessExp = Expression.LessThan(propertyExp, constValueExp);
                        expressions.Add(lessExp);
                        break;
                    case ConditionalOperatorType.LessThanOrEqual:
                        var lessOrEqualExp = Expression.LessThanOrEqual(propertyExp, constValueExp);
                        expressions.Add(lessOrEqualExp);
                        break;
                    default:
                        break;
                }
            }

            if (expressions.Count == 0)
            {
                return source;
            }

            var resultExp = expressions.Aggregate(Expression.OrElse);
            var lambda = Expression.Lambda(resultExp, false, parameterExp);
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { entityType }, source.Expression, lambda);
            return source.Provider.CreateQuery<TSource>(whereExpression);
        }


        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string key, OrderByType orderBy = OrderByType.Ascending)
        {
            try
            {
                /*
            *   Check if Key is null or empty space, or
            *   if someone has provided only the key 
            */
                string methodName = orderBy == OrderByType.Ascending ? "OrderBy" : "OrderByDescending";

                /*
                * Checking the type of TSource Object , for further processing
                */

                Type sourceType = typeof(TSource);

                /*
                * Creating Parameter Expression  t = > t , parameter  t will be of type TSource
                */

                var property = sourceType.GetProperty(key);


                if (property != null)
                {
                    var parameterExp = Expression.Parameter(sourceType, "t");

                    /*
                    * Getting Type of the Key t.Name
                    */

                    var memberExpression = Expression.PropertyOrField(parameterExp, property.Name);


                    var lambda = Expression.Lambda(memberExpression, new[] { parameterExp });

                    var sortExpression = Expression.Call(typeof(Queryable), methodName, new[] { sourceType, memberExpression.Type }, source.Expression, lambda);

                    return source.Provider.CreateQuery<TSource>(sortExpression);
                }

                throw new ArgumentNullException($"Unable to find Property with name ={key} ");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return source;
            }
        }

        private static Type GetPropertyType(Type type, string propName)
        {
            var property = type.GetProperty(propName);

            if (property == null)
            {
                return default;
            }

            return property.PropertyType;
        }

        private static bool IsNumericType(Type type)
        {
            HashSet<Type> numericTypes = new() { typeof(int), typeof(double), typeof(decimal), typeof(long), typeof(short), typeof(sbyte), typeof(byte), typeof(ulong), typeof(ushort), typeof(uint), typeof(float) };
            return numericTypes.Contains(type) || numericTypes.Contains(Nullable.GetUnderlyingType(type));
        }

        private static bool IsNumericValue(object value)
        {
            var x = value;
            return value is byte or short or int or long or sbyte or ushort or uint or ulong or decimal or double or float;
        }

        private static object GetMaxValueIfOutOfRange(object value, Type type)
        {
            if (!IsNumericValue(value) || !IsNumericType(type))
            {
                // no need to check in range if value or type is not numeric
                return value;
            }

            try
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GenericTypeArguments[0];
                }

                dynamic currentValue = value;
                dynamic maxValue = (type.GetField("MaxValue", BindingFlags.Public | BindingFlags.Static)).GetValue(null);
                if (currentValue > maxValue)
                {
                    return (ValueType)maxValue;
                }

                return value;
            }
            catch (Exception)
            {
                return value;
            }
        }

    }
}