namespace Zaman.Library.LINQExtensions.Constants;

public enum MethodType
{
    Empty = 0,
    StartsWith,
    DoesNotStartsWith,
    DoesNotEndsWith,
    EndsWith,
    Contains,
    DoesNotContains,
    Equal,
    NotEqual
}

public enum ArithmeticOperationType
{
    Sum,
    Multiply,
    Divide,
    Modulo,
    Subtract

}

public enum ConditionalOperatorType
{
    Empty,
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Between,
    NotBetween
}

public enum OrderByType
{
    Ascending,
    Descending
}