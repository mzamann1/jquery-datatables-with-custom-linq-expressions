using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;

namespace LINQExtensions.Models.ViewModels.JQueryDatatables;

public class JQueryDtRequest
{
    [JsonPropertyName("draw")]
    public int Draw { get; set; }

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("search")]
    public JQueryDtSearch Search { get; set; }

    [JsonPropertyName("order")]
    public JQueryDtOrder[] Order { get; set; }

    [JsonPropertyName("columns")]
    public JQueryDtColumn[] Columns { get; set; }

    [JsonPropertyName("searchBuilder")]
    public JQuerySearchBuilder? SearchBuilder { get; set; }


}

public class JQueryDtOrder
{
    [JsonPropertyName("column")]
    public int Column { get; set; }

    [JsonPropertyName("dir")]
    public string Dir { get; set; }
}

public class JQueryDtColumn
{
    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("searchable")]
    public bool Searchable { get; set; }

    [JsonPropertyName("orderable")]
    public bool Orderable { get; set; }

    [JsonPropertyName("search")]
    public JQueryDtSearch Search { get; set; }
}

public class JQueryDtSearch
{
    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("regex")]
    public bool Regex { get; set; }
}

public class JQuerySearchBuilder
{
    [JsonPropertyName("logic")]
    public string Logic { get; set; }

    [JsonPropertyName("criteria")]
    public JQuerySearchBuilderCriteria[] Criteria { get; set; }

}

public class JQuerySearchBuilderCriteria
{

    private string _condition;

    [JsonPropertyName("condition")]
    public string? Condition
    {
        get => _condition;
        set => _condition = GetConditions(value);
    }

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("origData")]
    public string? Origdata { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("value")]
    public string[]? Value { get; set; }

    [JsonPropertyName("value1")]
    public string? Value1 { get; set; }

    [JsonPropertyName("value2")]
    public string? Value2 { get; set; }



    private string GetConditions(string jqueryCondition)
    {
        return jqueryCondition switch
        {
            "=" => "Equal",
            "!=" => "NotEqual",
            "contains" => "Contains",
            "!contains" => "NotContain",
            "starts" => "StartsWith",
            "ends" => "EndsWith",
            _ => throw new ArgumentException("arguments not allowed")
        };
    }
}

//public enum JquerySearchBuilderCondition
//{

//    equal,
//    public static readonly string NotEqual = "!=";
//public static readonly string LessThan = "<";
//public static readonly string GreaterThan = ">";
//public static readonly string LessThanOrEqualTo = "<=";
//public static readonly string GreaterThanOrEqualTo = ">=";
//public static readonly string StartsWith = "starts";
//public static readonly string DoesNotStartsWith = "!starts";
//public static readonly string EndsWith = "ends";
//public static readonly string DoesNotEndsWith = "!ends";
//public static readonly string Contains = "contains";
//public static readonly string DoesNotContains = "!contains";
//}

