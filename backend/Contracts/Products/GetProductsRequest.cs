using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Products;

public record GetProductsRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    Guid? CategoryId = null,
    ProductSortBy SortBy = ProductSortBy.Name,
    SortDirection SortDirection = SortDirection.Asc);
