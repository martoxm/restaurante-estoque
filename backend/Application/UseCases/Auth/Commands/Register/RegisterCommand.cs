using MediatR;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.UseCases.Auth.Commands.Register;

public record RegisterCommand(string Name, string Email, string Password) : IRequest<LoginResponse>;
