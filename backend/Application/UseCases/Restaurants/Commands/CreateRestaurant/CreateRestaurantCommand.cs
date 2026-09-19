using MediatR;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Commands.CreateRestaurant;

public record CreateRestaurantCommand(string Name) : IRequest<Guid>;
