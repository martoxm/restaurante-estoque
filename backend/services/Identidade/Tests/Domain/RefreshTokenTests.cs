using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Tests.Domain;

public class RefreshTokenTests
{
    private static readonly DateTime Agora = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static RefreshToken CriarToken() =>
        RefreshToken.Criar(Guid.NewGuid(), "hash", Agora, Agora.AddDays(14));

    [Fact]
    public void Criar_SemUsuario_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => RefreshToken.Criar(Guid.Empty, "hash", Agora, Agora.AddDays(14)));
    }

    [Fact]
    public void Criar_ExpirandoAntesDeNascer_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => RefreshToken.Criar(Guid.NewGuid(), "hash", Agora, Agora));
    }

    [Fact]
    public void PodeSerUsado_TokenNovo_DeveSerVerdadeiro()
    {
        Assert.True(CriarToken().PodeSerUsado(Agora.AddMinutes(1)));
    }

    [Fact]
    public void PodeSerUsado_DepoisDeExpirar_DeveSerFalso()
    {
        Assert.False(CriarToken().PodeSerUsado(Agora.AddDays(14)));
    }

    [Fact]
    public void PodeSerUsado_RevogadoDentroDaTolerancia_DeveSerVerdadeiro()
    {
        var token = CriarToken();
        token.Revogar(Agora);

        Assert.True(token.PodeSerUsado(Agora.AddSeconds(60)));
        Assert.False(token.FoiReusadoForaDaTolerancia(Agora.AddSeconds(60)));
    }

    [Fact]
    public void PodeSerUsado_RevogadoForaDaTolerancia_DeveSerFalsoEApontarReuso()
    {
        var token = CriarToken();
        token.Revogar(Agora);

        Assert.False(token.PodeSerUsado(Agora.AddSeconds(61)));
        Assert.True(token.FoiReusadoForaDaTolerancia(Agora.AddSeconds(61)));
    }

    [Fact]
    public void Revogar_DuasVezes_DeveManterAPrimeiraData()
    {
        var token = CriarToken();
        token.Revogar(Agora);
        token.Revogar(Agora.AddMinutes(5));

        Assert.Equal(Agora, token.RevogadoEm);
    }
}
