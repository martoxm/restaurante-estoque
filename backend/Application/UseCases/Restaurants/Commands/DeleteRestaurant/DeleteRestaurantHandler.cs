using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Commands.DeleteRestaurant;

public class DeleteRestaurantHandler : IRequestHandler<DeleteRestaurantCommand>
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IApplicationDbContext _context;

    public DeleteRestaurantHandler(IRestaurantRepository restaurantRepository, IApplicationDbContext context)
    {
        _restaurantRepository = restaurantRepository;
        _context = context;
    }

    public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantRepository.GetByIdAsync(request.Id, cancellationToken);

        if (restaurant is null)
            throw new NotFoundException($"Restaurante com Id '{request.Id}' não encontrado.");

        _restaurantRepository.Remove(restaurant);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
