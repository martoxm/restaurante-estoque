using MediatR;
using RestauranteEstoque.Contracts.Restaurants;

namespace RestauranteEstoque.Application.UseCases.Restaurants.Queries.GetRestaurantById;

public record GetRestaurantByIdQuery(Guid Id) : IRequest<RestaurantDetailsDto?>;
