using RestauranteEstoque.Contracts.Common;
using System.Text.Json.Serialization;

namespace RestauranteEstoque.Contracts.Products;

public record GetProductsRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    Guid? CategoryId = null,
    ProductSortBy SortBy = ProductSortBy.Name,
    SortDirection SortDirection = SortDirection.Asc);

public record ProductCreatedResponse(Guid Id);

public record ProductDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int QuantityInStock,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt);

public record ProductListItemDto(
    Guid Id,
    string Name,
    decimal Price,
    int QuantityInStock,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProductSortBy
{
    Name,
    Price,
    QuantityInStock
}

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId);
