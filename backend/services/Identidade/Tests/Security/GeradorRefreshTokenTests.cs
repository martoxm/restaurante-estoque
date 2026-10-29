using Microsoft.Extensions.Options;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Identidade.Infrastructure.Security;

namespace RestauranteEstoque.Identidade.Tests.Security;

public class GeradorRefreshTokenTests
{
    private readonly GeradorRefreshToken _gerador = new(Options.Create(new JwtSettings { RefreshTokenDias = 14 }));

    [Fact]
    public void Gerar_DuasVezes_DeveDarTokensDiferentes()
    {
        Assert.NotEqual(_gerador.Gerar().Token, _gerador.Gerar().Token);
    }

    [Fact]
    public void Gerar_HashDeveTer64CaracteresEBaterComCalcularHash()
    {
        var gerado = _gerador.Gerar();

        Assert.Equal(64, gerado.Hash.Length);
        Assert.Equal(gerado.Hash, _gerador.CalcularHash(gerado.Token));
        Assert.NotEqual(gerado.Token, gerado.Hash);
    }
}
