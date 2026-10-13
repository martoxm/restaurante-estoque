using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Restaurants;

public record GetRestaurantsRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc);

public record RestaurantCreatedResponse(Guid Id);

public record RestaurantDetailsDto(Guid Id, string Name, DateTime CreatedAt);

public record RestaurantListItemDto(Guid Id, string Name, DateTime CreatedAt);

public record UpdateRestaurantRequest(string Name);
