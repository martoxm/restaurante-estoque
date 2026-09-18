using MediatR;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.UseCases.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
