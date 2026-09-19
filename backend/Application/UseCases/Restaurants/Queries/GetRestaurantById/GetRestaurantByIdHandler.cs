using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Restaurants;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdHandler : IRequestHandler<GetRestaurantByIdQuery, RestaurantDetailsDto?>
{
    private readonly IRestaurantRepository _restaurantRepository;

    public GetRestaurantByIdHandler(IRestaurantRepository restaurantRepository)
    {
        _restaurantRepository = restaurantRepository;
    }

    public async Task<RestaurantDetailsDto?> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(request.Id, cancellationToken);

        if (restaurant is null)
            return null;

        return new RestaurantDetailsDto(restaurant.Id, restaurant.Name, restaurant.CreatedAt);
    }
}
