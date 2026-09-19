using MediatR;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Commands.UpdateRestaurant;

public record UpdateRestaurantCommand(Guid Id, string Name) : IRequest;
