using MediatR;

namespace RestauranteEstoque.Application.UseCases.Restaurants;

public record CreateRestaurantCommand(string Name) : IRequest<Guid>;

public record DeleteRestaurantCommand(Guid Id) : IRequest;

public record UpdateRestaurantCommand(Guid Id, string Name) : IRequest;
