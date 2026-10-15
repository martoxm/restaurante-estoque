using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Restaurants;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Restaurants;

public class UpdateRestaurantHandler : IRequestHandler<UpdateRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IApplicationDbContext _context;

    public UpdateRestaurantHandler(IRestaurantRepository restaurantRepository, IApplicationDbContext context)
    {
        _restaurantRepository = restaurantRepository;
        _context = context;
    }

    public async Task Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(request.Id, cancellationToken);

        if (restaurant is null)
            throw new NotFoundException($"Restaurante com Id '{request.Id}' não encontrado.");

        restaurant.Update(request.Name);

        _restaurantRepository.Update(restaurant);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

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
