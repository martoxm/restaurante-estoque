using System.Text.Json.Serialization;

namespace RestauranteEstoque.Vendas.Contracts.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DirecaoOrdenacao
{
    Asc,
    Desc
}
