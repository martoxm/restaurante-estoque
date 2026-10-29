using System.Reflection;
using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Domain;

public class MovimentacaoEstoqueTests
{
    private static readonly DateTime DataMovimentacao = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarMovimentacao()
    {
        var produtoId = Guid.NewGuid();

        var movimentacao = MovimentacaoEstoque.Criar(DadosTeste.RestauranteId, produtoId, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(10), DataMovimentacao, "Compra do mês");

        Assert.Equal(produtoId, movimentacao.ProdutoId);
        Assert.Equal(TipoMovimentacaoEstoque.Entrada, movimentacao.Tipo);
        Assert.Equal(10, movimentacao.Quantidade.Valor);
        Assert.Equal("Compra do mês", movimentacao.Observacoes);
    }

    [Fact]
    public void Criar_ComProdutoIdVazio_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            MovimentacaoEstoque.Criar(DadosTeste.RestauranteId, Guid.Empty, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(10), DataMovimentacao));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Criar_ComQuantidadeZeroOuNegativa_DeveLancarDomainException(int quantidade)
    {
        Assert.Throws<DomainException>(() =>
            MovimentacaoEstoque.Criar(DadosTeste.RestauranteId, Guid.NewGuid(), TipoMovimentacaoEstoque.Saida, Quantidade.Criar(quantidade), DataMovimentacao));
    }

    [Fact]
    public void Criar_SemRestaurante_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            MovimentacaoEstoque.Criar(Guid.Empty, Guid.NewGuid(), TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(1), DataMovimentacao));
    }

    [Fact]
    public void Criar_NaoDeveSerPublico_ParaObrigarOUsoDoMovimentadorEstoque()
    {
        var criar = typeof(MovimentacaoEstoque).GetMethod(nameof(MovimentacaoEstoque.Criar), BindingFlags.Static | BindingFlags.NonPublic);

        Assert.NotNull(criar);
        Assert.True(criar!.IsAssembly);
    }
}
