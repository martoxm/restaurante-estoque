using System.Text.Json.Serialization;

namespace RestauranteEstoque.Contracts.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    Asc,
    Desc
}
