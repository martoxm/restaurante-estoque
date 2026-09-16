using System.Text.Json.Serialization;

namespace RestauranteEstoque.Contracts.Products;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductSortBy
{
    Name,
    Price,
    QuantityInStock
}
