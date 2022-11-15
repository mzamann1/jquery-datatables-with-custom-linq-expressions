using System.Linq.Expressions;
using UserManagement.Core.Constants;
using UserManagement.Core.Helpers;

namespace UserManagement.Core.Extensions;

public static class LINQExtension
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

    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string key, string value,
        MethodType methodType = MethodType.Equal)
    {
        /*
        *   Check if Key is null or empty space, or
        *   if someone has provided only the key 
        */
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value) && methodType != MethodType.Empty)
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

        var propertyType = Helper.GetPropertyType(sourceType, key);

        /*
        * Now we need to create expression for that property t=> x.name.tolowe()
        */

        var memberExp = sourceType.GetProperty(key) == null ? default : Expression.Property(parameterExp, key);

        /*
        * Now we need to convert the provided value's datatype to the property type from which it is going to be matched.
        */

        var toLowerExp = Expression.Call(memberExp,
            typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes));

        var convertedConstvalue = Expression.Convert(Expression.Constant(value.ToLower()), propertyType);

        Expression finalExp = default;

        switch (methodType)
        {

            case MethodType.Empty:
                finalExp = Expression.Call(typeof(string), nameof(string.IsNullOrWhiteSpace), null, toLowerExp);
                break;

            case MethodType.StartsWith:

                /*
                    * Getting StartsWith Method from string class using reflection
                */

                var startsWithInfo = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
                finalExp = Expression.Call(toLowerExp, startsWithInfo, convertedConstvalue);
                break;

            case MethodType.DoesNotStartsWith:

                /*
                    * Getting StartsWith Method from string class using reflection
                */

                var doesNotStartsWithInfo =
                    typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
                finalExp = Expression.Not(Expression.Call(toLowerExp, doesNotStartsWithInfo, convertedConstvalue));
                break;

            case MethodType.EndsWith:

                /*
                     * Getting EndsWith Method from string class using reflection
                */

                var endsWithInfo = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
                finalExp = Expression.Call(toLowerExp, endsWithInfo, convertedConstvalue);
                break;


            case MethodType.DoesNotEndsWith:

                /*
                     * Getting EndsWith Method from string class using reflection
                */

                var doesNotEndsWithInfo =
                    typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
                finalExp = Expression.Not(Expression.Call(toLowerExp, doesNotEndsWithInfo, convertedConstvalue));
                break;

            case MethodType.Contains:

                /*
                     * Getting EndsWith Method from string class using reflection
                */

                var containsInfo = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
                finalExp = Expression.Call(toLowerExp, containsInfo, convertedConstvalue);
                break;


            case MethodType.DoesNotContains:

                /*
                     * Getting EndsWith Method from string class using reflection
                */

                var doesNotContainsInfo =
                    typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
                finalExp = Expression.Not(Expression.Call(toLowerExp, doesNotContainsInfo, convertedConstvalue));
                break;

            case MethodType.Equal:

                finalExp = Expression.Equal(toLowerExp, convertedConstvalue);
                break;

            case MethodType.NotEqual:

                finalExp = Expression.NotEqual(toLowerExp, convertedConstvalue);
                break;

            default:
                return source;
        }

        var lambda = Expression.Lambda(finalExp, false, parameterExp);
        var whereExpression =
            Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

        return source.Provider.CreateQuery<TSource>(whereExpression);
    }

    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string key, object value,
        object value2 = null, ConditionalOperatorType operatorType = ConditionalOperatorType.Empty)
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

        var propertyType = Helper.GetPropertyType(sourceType, key);

        /*
        * Now we need to create expression for that property
        */

        var memberExp = sourceType.GetProperty(key) == null ? default : Expression.Property(parameterExp, key);

        /*
        * Now we need to convert the provided value's datatype to the property type from which it is going to be matched.
        */

        UnaryExpression? convertedFisrtValue = null;

        if (operatorType == ConditionalOperatorType.Between || operatorType == ConditionalOperatorType.NotBetween)
        {
            if (!Helper.IsNumericType(memberExp.Type) ||
                (!Helper.IsNumericValue(value) && !Helper.IsNumericValue(value2)))
            {
                return source;
            }
        }
        else
        {
            if (!Helper.IsNumericType(memberExp.Type) || !Helper.IsNumericValue(value))
            {
                return source;
            }

            convertedFisrtValue = Expression.Convert(Expression.Constant(value), propertyType);
        }

        Expression? finalExp = default;

        switch (operatorType)
        {

            case ConditionalOperatorType.Empty:
                return source;

            case ConditionalOperatorType.Equal:

                finalExp = Expression.Equal(memberExp, convertedFisrtValue);
                break;

            case ConditionalOperatorType.NotEqual:

                finalExp = Expression.NotEqual(memberExp, convertedFisrtValue);
                break;

            case ConditionalOperatorType.GreaterThan:

                finalExp = Expression.GreaterThan(memberExp, convertedFisrtValue);
                break;

            case ConditionalOperatorType.GreaterThanOrEqual:

                finalExp = Expression.GreaterThanOrEqual(memberExp, convertedFisrtValue);
                break;

            case ConditionalOperatorType.LessThan:

                finalExp = Expression.LessThan(memberExp, convertedFisrtValue);
                break;

            case ConditionalOperatorType.LessThanOrEqual:

                finalExp = Expression.LessThanOrEqual(memberExp, convertedFisrtValue);
                break;

            case ConditionalOperatorType.Between:

                if (value != null && value2 == null)
                {
                    finalExp = Expression.GreaterThan(memberExp,
                        Expression.Convert(Expression.Constant(value), propertyType));
                    break;
                }
                else if (value2 != null && value == null)
                {
                    finalExp = Expression.LessThan(memberExp,
                        Expression.Convert(Expression.Constant(value2), propertyType));
                    break;
                }
                else if (value2 is not null && value is not null)
                {
                    finalExp = Expression.And(
                        Expression.GreaterThan(memberExp, Expression.Convert(Expression.Constant(value), propertyType)),
                        Expression.LessThan(memberExp, Expression.Convert(Expression.Constant(value2), propertyType)));
                    break;
                }
                else
                {
                    return source;
                }

            case ConditionalOperatorType.NotBetween:

                finalExp = Expression.Not(Expression.And(
                    Expression.GreaterThan(memberExp, Expression.Convert(Expression.Constant(value2), propertyType)),
                    Expression.LessThan(memberExp, convertedFisrtValue)));
                break;

            default:
                break;
        }

        var lambda = Expression.Lambda(finalExp, false, parameterExp);
        var whereExpression =
            Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

        return source.Provider.CreateQuery<TSource>(whereExpression);
    }

    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string key, object value1,
        object value2, bool negate)
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

        var propertyType = Helper.GetPropertyType(sourceType, key);

        /*
        * Now we need to create expression for that property
        */

        var memberExp = sourceType.GetProperty(key) == null ? default : Expression.Property(parameterExp, key);

        /*
        * Now we need to convert the provided value's datatype to the property type from which it is going to be matched.
        */


        if (!Helper.IsNumericType(memberExp.Type) || !Helper.IsNumericValue(value1) || !Helper.IsNumericValue(value2))
        {
            return source;
        }

        var convertedFirstvalue = Expression.Convert(Expression.Constant(value1), propertyType);
        var convertedSecondValue = Expression.Convert(Expression.Constant(value2), propertyType);

        Expression finalExp = default;

        if (negate)
        {
            finalExp = Expression.Not(Expression.And(Expression.GreaterThan(memberExp, convertedFirstvalue),
                Expression.LessThan(memberExp, convertedSecondValue)));
        }
        else
        {
            finalExp = Expression.And(Expression.GreaterThan(memberExp, convertedFirstvalue),
                Expression.LessThan(memberExp, convertedSecondValue));
        }


        var lambda = Expression.Lambda(finalExp, false, parameterExp);
        var whereExpression =
            Expression.Call(typeof(Queryable), "Where", new[] { sourceType }, source.Expression, lambda);

        return source.Provider.CreateQuery<TSource>(whereExpression);
    }

    //public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, IEnumerable<DtColumn> searachableCols, string value, MethodType methodType = MethodType.Equal)
    //{
    //    if (string.IsNullOrEmpty(value) && methodType != MethodType.Empty)
    //    {
    //        return source;
    //    }

    //    var containsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
    //    var endsWithMethodInfo = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
    //    var toLowerMethodInfo = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
    //    var startsWithMethodInfo = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
    //    var trimMethodInfo = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes);

    //    Type entityType = typeof(TSource);
    //    var parameterExp = Expression.Parameter(entityType, "x");
    //    var expressions = new List<Expression>();


    //    var properties = entityType.GetProperties().Join(searachableCols, prop1 => prop1.Name, prop2 => prop2.Name, (prop1, prop2) => new
    //    {
    //        prop1.PropertyType,
    //        prop1.Name
    //    });



    //    foreach (var property in properties)
    //    {
    //        if (!property.PropertyType.Equals(typeof(string)))
    //        {
    //            continue;
    //        }

    //        var propertyExp = Expression.PropertyOrField(parameterExp, property.Name);
    //        var constValueExp = Expression.Convert(Expression.Constant(value.ToLower()), property.PropertyType);

    //        var toLowerExp = Expression.Call(propertyExp,
    //            toLowerMethodInfo);

    //        switch (methodType)
    //        {
    //            case MethodType.Empty:
    //                // Translate to Expression
    //                // x.Property == null || x.Property.Trim() == string.Empty
    //                var constNullExp = Expression.Convert(Expression.Constant(null), property.PropertyType);
    //                var constEmptyExp = Expression.Convert(Expression.Constant(string.Empty), property.PropertyType);
    //                var equalToNullExp = Expression.Equal(toLowerExp, constNullExp);
    //                var equalToEmptyExp = Expression.Equal(Expression.Call(toLowerExp, trimMethodInfo), constEmptyExp);
    //                var blankExp = Expression.OrElse(equalToNullExp, equalToEmptyExp);

    //                // string.IsNullOrWhiteSpace(x.Property)
    //                // Performance wise, this method is faster..
    //                var isNullOrWhiteSpaceExp = Expression.Call(typeof(string), nameof(string.IsNullOrWhiteSpace), null, toLowerExp);
    //                expressions.Add(isNullOrWhiteSpaceExp);
    //                break;
    //            case MethodType.Equal:
    //                var equalExp = Expression.Equal(toLowerExp, constValueExp);
    //                expressions.Add(equalExp);
    //                break;
    //            case MethodType.NotEqual:
    //                var notEqualExp = Expression.NotEqual(toLowerExp, constValueExp);
    //                expressions.Add(notEqualExp);
    //                break;
    //            case MethodType.Contains:
    //                var containsExp = Expression.Call(toLowerExp, containsMethodInfo, constValueExp);
    //                expressions.Add(containsExp);
    //                break;
    //            case MethodType.StartsWith:
    //                var startsWithExp = Expression.Call(toLowerExp, startsWithMethodInfo, constValueExp);
    //                expressions.Add(startsWithExp);
    //                break;
    //            case MethodType.EndsWith:
    //                var endsWithExp = Expression.Call(toLowerExp, endsWithMethodInfo, constValueExp);
    //                expressions.Add(endsWithExp);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    if (expressions.Count == 0)
    //    {
    //        return source;
    //    }

    //    var resultExp = expressions.Aggregate(Expression.OrElse);
    //    var lambda = Expression.Lambda(resultExp, false, parameterExp);
    //    var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { entityType }, source.Expression, lambda);
    //    return source.Provider.CreateQuery<TSource>(whereExpression);
    //}

    //public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, IEnumerable<DtColumn> searachableCols, object value, ConditionalOperatorType condition = ConditionalOperatorType.Equals)
    //{
    //    if (value == null || !Helper.IsNumericValue(value) && condition != ConditionalOperatorType.Empty)
    //    {
    //        return source;
    //    }

    //    Type entityType = typeof(TSource);
    //    var parameterExp = Expression.Parameter(entityType, "x");
    //    var expressions = new List<Expression>();

    //    var properties = entityType.GetProperties().Join(searachableCols, prop1 => prop1.Name, prop2 => prop2.Name, (prop1, prop2) => new
    //    {
    //        prop1.PropertyType,
    //        prop1.Name
    //    });


    //    foreach (var property in properties)
    //    {
    //        if (!Helper.IsNumericType(property.PropertyType))
    //        {
    //            continue;
    //        }

    //        var propertyExp = Expression.PropertyOrField(parameterExp, property.Name);
    //        var constValueExp = Expression.Convert(Expression.Constant(Helper.GetMaxValueIfOutOfRange(value, property.PropertyType)), property.PropertyType);
    //        switch (condition)
    //        {
    //            case ConditionalOperatorType.Empty:
    //                if (Nullable.GetUnderlyingType(property.PropertyType) == null)
    //                {
    //                    // Property is not nullable ..
    //                    continue;
    //                }

    //                var equalToNullExp = Expression.Equal(propertyExp, Expression.Convert(Expression.Constant(null), property.PropertyType));
    //                expressions.Add(equalToNullExp);
    //                break;
    //            case ConditionalOperatorType.Equals:
    //                var equalExp = Expression.Equal(propertyExp, constValueExp);
    //                expressions.Add(equalExp);
    //                break;
    //            case ConditionalOperatorType.NotEquals:
    //                var notEqualExp = Expression.NotEqual(propertyExp, constValueExp);
    //                expressions.Add(notEqualExp);
    //                break;
    //            case ConditionalOperatorType.GreaterThan:
    //                var greaterExp = Expression.GreaterThan(propertyExp, constValueExp);
    //                expressions.Add(greaterExp);
    //                break;
    //            case ConditionalOperatorType.GreaterThanOrEqual:
    //                var greaterOrEqualExp = Expression.GreaterThanOrEqual(propertyExp, constValueExp);
    //                expressions.Add(greaterOrEqualExp);
    //                break;
    //            case ConditionalOperatorType.LessThan:
    //                var lessExp = Expression.LessThan(propertyExp, constValueExp);
    //                expressions.Add(lessExp);
    //                break;
    //            case ConditionalOperatorType.LessThanOrEqual:
    //                var lessOrEqualExp = Expression.LessThanOrEqual(propertyExp, constValueExp);
    //                expressions.Add(lessOrEqualExp);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    if (expressions.Count == 0)
    //    {
    //        return source;
    //    }

    //    var resultExp = expressions.Aggregate(Expression.OrElse);
    //    var lambda = Expression.Lambda(resultExp, false, parameterExp);
    //    var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { entityType }, source.Expression, lambda);
    //    return source.Provider.CreateQuery<TSource>(whereExpression);
    //}


    //IENuerable string

    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source,
        IEnumerable<string> searachableCols, string value, MethodType methodType = MethodType.Equal)
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


        var properties = entityType.GetProperties().Join(searachableCols, prop1 => prop1.Name, prop2 => prop2,
            (prop1, prop2) => new
            {
                PropertyType = prop1.PropertyType,
                Name = prop1.Name,
                ColName = prop2
            });



        foreach (var property in properties)
        {
            if (!property.PropertyType.Equals(typeof(string)))
            {
                continue;
            }

            var propertyExp = Expression.PropertyOrField(parameterExp, property.Name);
            var constValueExp = Expression.Convert(Expression.Constant(value.ToLower()), property.PropertyType);

            var toLowerExp = Expression.Call(propertyExp,
                toLowerMethodInfo);

            switch (methodType)
            {
                case MethodType.Empty:
                    // Translate to Expression
                    // x.Property == null || x.Property.Trim() == string.Empty
                    var constNullExp = Expression.Convert(Expression.Constant(null), property.PropertyType);
                    var constEmptyExp = Expression.Convert(Expression.Constant(string.Empty), property.PropertyType);
                    var equalToNullExp = Expression.Equal(toLowerExp, constNullExp);
                    var equalToEmptyExp = Expression.Equal(Expression.Call(toLowerExp, trimMethodInfo), constEmptyExp);
                    var blankExp = Expression.OrElse(equalToNullExp, equalToEmptyExp);

                    // string.IsNullOrWhiteSpace(x.Property)
                    // Performance wise, this method is faster..
                    var isNullOrWhiteSpaceExp = Expression.Call(typeof(string), nameof(string.IsNullOrWhiteSpace), null,
                        toLowerExp);
                    expressions.Add(isNullOrWhiteSpaceExp);
                    break;
                case MethodType.Equal:
                    var equalExp = Expression.Equal(toLowerExp, constValueExp);
                    expressions.Add(equalExp);
                    break;
                case MethodType.NotEqual:
                    var notEqualExp = Expression.NotEqual(toLowerExp, constValueExp);
                    expressions.Add(notEqualExp);
                    break;
                case MethodType.Contains:
                    var containsExp = Expression.Call(toLowerExp, containsMethodInfo, constValueExp);
                    expressions.Add(containsExp);
                    break;
                case MethodType.StartsWith:
                    var startsWithExp = Expression.Call(toLowerExp, startsWithMethodInfo, constValueExp);
                    expressions.Add(startsWithExp);
                    break;
                case MethodType.EndsWith:
                    var endsWithExp = Expression.Call(toLowerExp, endsWithMethodInfo, constValueExp);
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
        var whereExpression =
            Expression.Call(typeof(Queryable), "Where", new[] { entityType }, source.Expression, lambda);
        return source.Provider.CreateQuery<TSource>(whereExpression);
    }

    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source,
        IEnumerable<string> searachableCols, object value,
        ConditionalOperatorType condition = ConditionalOperatorType.Equal)
    {
        if (value == null || !Helper.IsNumericValue(value) && condition != ConditionalOperatorType.Empty)
        {
            return source;
        }

        Type entityType = typeof(TSource);
        var parameterExp = Expression.Parameter(entityType, "x");
        var expressions = new List<Expression>();

        var properties = entityType.GetProperties().Join(searachableCols, prop1 => prop1.Name, prop2 => prop2,
            (prop1, prop2) => new
            {
                PropertyType = prop1.PropertyType,
                Name = prop1.Name,
            });


        foreach (var property in properties)
        {
            if (!Helper.IsNumericType(property.PropertyType))
            {
                continue;
            }

            var propertyExp = Expression.PropertyOrField(parameterExp, property.Name);
            var constValueExp =
                Expression.Convert(Expression.Constant(Helper.GetMaxValueIfOutOfRange(value, property.PropertyType)),
                    property.PropertyType);
            switch (condition)
            {
                case ConditionalOperatorType.Empty:
                    if (Nullable.GetUnderlyingType(property.PropertyType) == null)
                    {
                        // Property is not nullable ..
                        continue;
                    }

                    var equalToNullExp = Expression.Equal(propertyExp,
                        Expression.Convert(Expression.Constant(null), property.PropertyType));
                    expressions.Add(equalToNullExp);
                    break;
                case ConditionalOperatorType.Equal:
                    var equalExp = Expression.Equal(propertyExp, constValueExp);
                    expressions.Add(equalExp);
                    break;
                case ConditionalOperatorType.NotEqual:
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
        var whereExpression =
            Expression.Call(typeof(Queryable), "Where", new[] { entityType }, source.Expression, lambda);
        return source.Provider.CreateQuery<TSource>(whereExpression);
    }
    //public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, DtRequestModel inParam)
    //{

    //    if (string.IsNullOrWhiteSpace(inParam.Search.Value) && inParam.Columns.All(x => string.IsNullOrWhiteSpace(x.Search.Value)))
    //    {
    //        return source;
    //    }

    //    var searchableCols = inParam.Columns.Where(x => x.Searchable);

    //    int searchCount = searchableCols.Count();

    //    var sourceType = typeof(TSource);

    //    //need to check again
    //    var properties = sourceType.GetProperties().Join(searchableCols, prop1 => prop1.Name, prop2 => prop2.Name, (prop1, prop2) => new
    //    {
    //        prop1.PropertyType,
    //        prop1.Name,
    //        prop2.Search.Value
    //    });


    //    if (searchCount > 0 && searchableCols.Any(x => !string.IsNullOrWhiteSpace(x.Search.Value)))
    //    {
    //        foreach (var prop in properties)
    //        {
    //            if (Helper.IsNumericType(prop.PropertyType))
    //            {
    //                bool valueIsNumeric = decimal.TryParse(prop.Value, out decimal numericValue);
    //                if (valueIsNumeric)
    //                {
    //                    source = source.Where(prop.Name, numericValue);

    //                }
    //            }
    //            else
    //            {
    //                source = source.Where(prop.Name, prop.Value);
    //            }

    //        }

    //    }


    //    if (searchCount > 0 && !string.IsNullOrWhiteSpace(inParam.Search.Value))
    //    {

    //        foreach (var prop in properties)
    //        {
    //            if (Helper.IsNumericType(prop.PropertyType))
    //            {
    //                bool valueIsNumeric = decimal.TryParse(inParam.Search.Value, out decimal numericValue);
    //                if (valueIsNumeric)
    //                {
    //                    source = source.Where(searchableCols, numericValue);
    //                }
    //            }
    //            else
    //            {
    //                source = source.Where(searchableCols, inParam.Search.Value, MethodType.Contains);
    //            }

    //        };
    //    }
    //    return source;
    //}


    public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string key,
        OrderByType orderBy = OrderByType.Ascending)
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

                var sortExpression = Expression.Call(typeof(Queryable), methodName,
                    new[] { sourceType, memberExpression.Type }, source.Expression, lambda);

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

}