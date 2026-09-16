using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Categories;

public record GetCategoriesRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc);
