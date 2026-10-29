using MediatR;
using RestauranteEstoque.Identidade.Contracts.Auth;

namespace RestauranteEstoque.Identidade.Application.UseCases.Auth;

public record LoginCommand(string Email, string Senha) : IRequest<LoginResponse>;

public record RegistrarCommand(string Nome, string Email, string Senha, string NomeRestaurante) : IRequest<LoginResponse>;

public record RenovarTokenCommand(string RefreshToken) : IRequest<LoginResponse>;
