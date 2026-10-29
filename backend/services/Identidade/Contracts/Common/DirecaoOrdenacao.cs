using System.Text.Json.Serialization;

namespace RestauranteEstoque.Identidade.Contracts.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DirecaoOrdenacao
{
    Asc,
    Desc
}
