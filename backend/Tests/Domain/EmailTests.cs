using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Domain;

public class EmailTests
{
    [Fact]
    public void Criar_DeveRemoverEspacosEDeixarMinusculo()
    {
        var email = Email.Criar("  Contato@ABC.com  ");

        Assert.Equal("contato@abc.com", email.Valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("sem-arroba")]
    [InlineData("a@b")]
    [InlineData("com espaco@abc.com")]
    public void Criar_ComEmailInvalido_DeveLancarDomainException(string valorInvalido)
    {
        Assert.Throws<DomainException>(() => Email.Criar(valorInvalido));
    }

    [Fact]
    public void Criar_ComMaisDeDuzentosCaracteres_DeveLancarDomainException()
    {
        var longo = new string('a', 195) + "@abc.com";

        Assert.Throws<DomainException>(() => Email.Criar(longo));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CriarOuNulo_ComTextoVazio_DeveDevolverNull(string? valor)
    {
        Assert.Null(Email.CriarOuNulo(valor));
    }

    [Fact]
    public void EmailsComMesmoEnderecoEmCaixaDiferente_DevemSerIguais()
    {
        Assert.Equal(Email.Criar("Contato@abc.com"), Email.Criar("contato@ABC.com"));
    }
}
