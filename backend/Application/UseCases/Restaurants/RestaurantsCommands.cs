using MediatR;

namespace RestauranteEstoque.Application.UseCases.Restaurants;

public record UpdateRestaurantCommand(Guid Id, string Name) : IRequest;
