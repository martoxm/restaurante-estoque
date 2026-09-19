using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Restaurants;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Queries.GetRestaurants;

public record GetRestaurantsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc) : IRequest<PagedResult<RestaurantListItemDto>>;
