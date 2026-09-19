using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Restaurants;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Queries.GetRestaurants;

public class GetRestaurantsHandler : IRequestHandler<GetRestaurantsQuery, PagedResult<RestaurantListItemDto>>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public GetRestaurantsHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<PagedResult<RestaurantListItemDto>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var (restaurants, totalCount) = await _restaurantRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Name,
            request.SortDirection,
            cancellationToken);

        var items = restaurants
            .Select(restaurant => new RestaurantListItemDto(restaurant.Id, restaurant.Name, restaurant.CreatedAt))
            .ToList();

        return new PagedResult<RestaurantListItemDto>(items, request.Page, request.PageSize, totalCount);
    }
}
