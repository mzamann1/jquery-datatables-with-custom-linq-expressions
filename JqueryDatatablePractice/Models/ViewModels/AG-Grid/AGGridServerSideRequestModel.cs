using System.Text.Json.Serialization;

namespace JqueryDatatablePractice.Models.ViewModels.AG_Grid
{

    public class AGGridServerSideRequestModel
    {
        [JsonPropertyName("startRow")]
        public int? StartRow { get; set; }

        [JsonPropertyName("endRow")]
        public int? EndRow { get; set; }

        [JsonPropertyName("rowGroupCols")]
        public ColumnVO[] RowGroupCols { get; set; }

        [JsonPropertyName("valueCols")]
        public ColumnVO[] ValueCols { get; set; }

        [JsonPropertyName("pivotCols")]
        public ColumnVO[] PivotCols { get; set; }

        [JsonPropertyName("pivotModel")]
        public bool PivotModel { get; set; }

        [JsonPropertyName("groupKeys")]
        public string[] GroupKeys { get; set; }

        [JsonPropertyName("filterModel")]
        public object FilterModel { get; set; }

        [JsonPropertyName("sortModel")]
        public SortModelItem[] SortModel { get; set; }
    }


    public class ColumnVO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("aggFunc")]
        public string AggFunc { get; set; }
    }

    public class SortModelItem
    {
        [JsonPropertyName("colId")]
        public string ColId { get; set; }

        [JsonPropertyName("sort")]
        public SortOrder Sort { get; set; }
    }


    public enum SortOrder
    {
        asc,
        desc
    }
}
