using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Domain;

public class QuantidadeTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(15)]
    public void Criar_ComValorZeroOuPositivo_DeveGuardarOValor(int valor)
    {
        Assert.Equal(valor, Quantidade.Criar(valor).Valor);
    }

    [Fact]
    public void Criar_ComValorNegativo_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => Quantidade.Criar(-1));
    }

    [Fact]
    public void Somar_DeveDevolverNovaQuantidadeSemAlterarAOriginal()
    {
        var original = Quantidade.Criar(5);

        var resultado = original.Somar(Quantidade.Criar(3));

        Assert.Equal(8, resultado.Valor);
        Assert.Equal(5, original.Valor);
    }

    [Fact]
    public void Subtrair_DeveDevolverADiferenca()
    {
        Assert.Equal(2, Quantidade.Criar(5).Subtrair(Quantidade.Criar(3)).Valor);
    }

    [Fact]
    public void Subtrair_QueDeixariaNegativo_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => Quantidade.Criar(2).Subtrair(Quantidade.Criar(3)));
    }

    [Fact]
    public void Operadores_DeComparacao_DevemCompararPeloValor()
    {
        Assert.True(Quantidade.Criar(5) > Quantidade.Criar(3));
        Assert.True(Quantidade.Criar(3) < Quantidade.Criar(5));
        Assert.True(Quantidade.Criar(5) >= Quantidade.Criar(5));
        Assert.Equal(Quantidade.Criar(5), Quantidade.Criar(5));
    }
}
