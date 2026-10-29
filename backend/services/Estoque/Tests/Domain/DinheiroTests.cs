using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Domain;

public class DinheiroTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(25.90)]
    public void Criar_ComValorZeroOuPositivo_DeveGuardarOValor(decimal valor)
    {
        var dinheiro = Dinheiro.Criar(valor);

        Assert.Equal(valor, dinheiro.Valor);
    }

    [Fact]
    public void Criar_ComValorNegativo_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => Dinheiro.Criar(-0.01m));
    }

    [Fact]
    public void DoisDinheirosComOMesmoValor_DevemSerIguais()
    {
        Assert.Equal(Dinheiro.Criar(10m), Dinheiro.Criar(10m));
        Assert.True(Dinheiro.Criar(10m) == Dinheiro.Criar(10m));
        Assert.NotEqual(Dinheiro.Criar(10m), Dinheiro.Criar(11m));
    }

    [Fact]
    public void CompareTo_DeveOrdenarPeloValor()
    {
        var ordenados = new[] { Dinheiro.Criar(30m), Dinheiro.Criar(10m), Dinheiro.Criar(20m) }
            .Order()
            .Select(dinheiro => dinheiro.Valor);

        Assert.Equal(new[] { 10m, 20m, 30m }, ordenados);
    }
}
