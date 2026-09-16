using MediatR;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
