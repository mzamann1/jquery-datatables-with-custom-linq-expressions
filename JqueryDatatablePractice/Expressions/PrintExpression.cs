using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQExtensions.Expressions;

public static class PrintExpression
{
    public static void PrintConsoleMessage(string message)
    {

        ParameterExpression paramExp = Expression.Parameter(typeof(string));

        /*
         * Getting Method Call Expression
         */

        MethodCallExpression methodCall = Expression.Call(
            typeof(Console).GetMethod(nameof(Console.WriteLine), new Type[] { typeof(string) }), paramExp);

        /*
         * Using Action, because no return type is required
         */

        Expression<Action<string>> printMessageExp = Expression.Lambda<Action<string>>(
            methodCall,
            new ParameterExpression[] { paramExp });

        Action<string> printMessage = printMessageExp.Compile();

        printMessage(message);
    }

    public static void PrintNumber(int numberParam)
    {
        ParameterExpression paramExp = Expression.Parameter(typeof(int));

        MethodCallExpression methodCall = Expression.Call(
            typeof(Console).GetMethod(nameof(Console.WriteLine), new Type[] { typeof(int) }), paramExp);

        Action<int> printInt = Expression.Lambda<Action<int>>(
            methodCall,
            new ParameterExpression[] { paramExp }).Compile();

        printInt(100);
    }

}


