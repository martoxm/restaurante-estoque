using MediatR;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.Auth.Commands.Register;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<LoginResponse>;
