using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Suppliers;

public record GetSuppliersRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc);
