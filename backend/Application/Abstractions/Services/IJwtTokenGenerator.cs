namespace RestauranteEstoque.Application.Abstractions.Services;

public interface IJwtTokenGenerator
{
    string GerarToken(Guid usuarioId, string nome, string email, IEnumerable<string> roles, Guid? restauranteId);
}
