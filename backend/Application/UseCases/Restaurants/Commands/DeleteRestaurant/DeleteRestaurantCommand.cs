using MediatR;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Commands.DeleteRestaurant;

public record DeleteRestaurantCommand(Guid Id) : IRequest;
