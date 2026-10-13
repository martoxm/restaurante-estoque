using MediatR;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.UseCases.Auth;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;

public record RegisterCommand(string Name, string Email, string Password, string RestaurantName) : IRequest<LoginResponse>;
