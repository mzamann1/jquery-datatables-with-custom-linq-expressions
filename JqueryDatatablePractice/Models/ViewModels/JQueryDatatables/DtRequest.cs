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
    public JQueryDtColumn [] Columns { get; set; }
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


