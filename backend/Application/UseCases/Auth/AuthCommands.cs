using MediatR;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.UseCases.Auth;

public record LoginCommand(string Email, string Senha) : IRequest<LoginResponse>;

public record RegistrarCommand(string Nome, string Email, string Senha, string NomeRestaurante) : IRequest<LoginResponse>;
