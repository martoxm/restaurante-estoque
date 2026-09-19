using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Restaurants;

public record GetRestaurantsRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc);
