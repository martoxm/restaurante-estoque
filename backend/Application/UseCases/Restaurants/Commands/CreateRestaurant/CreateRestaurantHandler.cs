using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantHandler : IRequestHandler<CreateRestaurantCommand, Guid>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IApplicationDbContext _context;
    private readonly IClock _clock;

    public CreateRestaurantHandler(IRestaurantRepository restaurantRepository, IApplicationDbContext context, IClock clock)
    {
        _restaurantRepository = restaurantRepository;
        _context = context;
        _clock = clock;
    }

    public async Task<Guid> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = Restaurant.Create(request.Name, _clock.UtcNow);

        _restaurantRepository.Add(restaurant);
        await _context.SaveChangesAsync(cancellationToken);

        return restaurant.Id;
    }
}
