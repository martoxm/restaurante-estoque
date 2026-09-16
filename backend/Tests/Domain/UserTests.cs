using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_ComDadosValidos_DeveCriarUsuario()
    {
        var user = User.Create("Gabriel", "gabriel@restaurante.com", "hash-fake-de-senha");

        Assert.Equal("Gabriel", user.Name);
        Assert.Equal("gabriel@restaurante.com", user.Email);
        Assert.Equal("hash-fake-de-senha", user.PasswordHash);
    }

    [Fact]
    public void Create_DeveNormalizarEmailParaMinusculoESemEspacos()
    {
        var user = User.Create("Gabriel", "  GABRIEL@Restaurante.COM  ", "hash-fake");

        Assert.Equal("gabriel@restaurante.com", user.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ComNomeInvalido_DeveLancarDomainException(string? nomeInvalido)
    {
        Assert.Throws<DomainException>(() =>
            User.Create(nomeInvalido!, "gabriel@restaurante.com", "hash-fake"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-arroba.com")]
    [InlineData("sem-ponto@com")]
    public void Create_ComEmailInvalido_DeveLancarDomainException(string emailInvalido)
    {
        Assert.Throws<DomainException>(() =>
            User.Create("Gabriel", emailInvalido, "hash-fake"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ComPasswordHashInvalido_DeveLancarDomainException(string? hashInvalido)
    {
        Assert.Throws<DomainException>(() =>
            User.Create("Gabriel", "gabriel@restaurante.com", hashInvalido!));
    }
}
