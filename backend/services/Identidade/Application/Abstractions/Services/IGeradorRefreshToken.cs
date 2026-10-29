namespace RestauranteEstoque.Identidade.Application.Abstractions.Services;

public record RefreshTokenGerado(string Token, string Hash);

public interface IGeradorRefreshToken
{
    int DiasValidade { get; }

    RefreshTokenGerado Gerar();

    string CalcularHash(string token);
}
