using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JqueryDatatablePractice.Constants;

namespace JqueryDatatablePractice.Expressions;

public static class ArithmeticOperationExpression
{
    public static int ApplyArithmeticOperation(int a, int b, ArithmeticOperationType op = ArithmeticOperationType.Sum)
    {
        //defining expression
        
        ParameterExpression param1 = Expression.Parameter(typeof(int));
        ParameterExpression param2 = Expression.Parameter(typeof(int));

        BinaryExpression binaryExp = null;

        switch (op)
        {
            case ArithmeticOperationType.Sum:
                binaryExp = Expression.Add(param1, param2);
                break;
            case ArithmeticOperationType.Subtract:
                binaryExp = Expression.Subtract(param1, param2);
                break;
            case ArithmeticOperationType.Multiply:
                binaryExp = Expression.Multiply(param1, param2);
                break;
            case ArithmeticOperationType.Divide:
                binaryExp = Expression.Divide(param1, param2);
                break;
            case ArithmeticOperationType.Modulo:
                binaryExp = Expression.Modulo(param1, param2);
                break;
            default:
                break;
        }

        if (binaryExp == null)
        {
            return 0;
        }

        Expression<Func<int, int, int>> lambda = Expression.Lambda<Func<int, int, int>>(binaryExp, param1, param2);

        //compiling expression
        Func<int, int, int> compiledLambda = lambda.Compile();

        //running expression
        return compiledLambda(a, b);
    }

}

