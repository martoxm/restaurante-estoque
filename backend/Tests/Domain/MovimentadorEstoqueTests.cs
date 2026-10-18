using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Domain.Services;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Domain;

public class MovimentadorEstoqueTests
{
    private static readonly DateTime Data = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static Produto CriarProdutoComEstoque(int quantidade) =>
        Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), Guid.NewGuid(), Data, quantidadeInicial: Quantidade.Criar(quantidade));

    [Fact]
    public void Movimentar_Entrada_DeveAumentarEstoqueERegistrarMovimentacao()
    {
        var produto = CriarProdutoComEstoque(10);

        var movimentacao = MovimentadorEstoque.Movimentar(produto, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(5), Data, "Compra");

        Assert.Equal(15, produto.QuantidadeEmEstoque.Valor);
        Assert.Equal(produto.Id, movimentacao.ProdutoId);
        Assert.Equal(TipoMovimentacaoEstoque.Entrada, movimentacao.Tipo);
        Assert.Equal(5, movimentacao.Quantidade.Valor);
        Assert.Equal(Data, movimentacao.DataMovimentacao);
    }

    [Fact]
    public void Movimentar_Saida_DeveDiminuirEstoque()
    {
        var produto = CriarProdutoComEstoque(10);

        var movimentacao = MovimentadorEstoque.Movimentar(produto, TipoMovimentacaoEstoque.Saida, Quantidade.Criar(4), Data);

        Assert.Equal(6, produto.QuantidadeEmEstoque.Valor);
        Assert.Equal(TipoMovimentacaoEstoque.Saida, movimentacao.Tipo);
    }

    [Fact]
    public void Movimentar_SaidaMaiorQueEstoque_DeveLancarDomainExceptionSemAlterarEstoque()
    {
        var produto = CriarProdutoComEstoque(3);

        Assert.Throws<DomainException>(() =>
            MovimentadorEstoque.Movimentar(produto, TipoMovimentacaoEstoque.Saida, Quantidade.Criar(5), Data));

        Assert.Equal(3, produto.QuantidadeEmEstoque.Valor);
    }

    [Fact]
    public void Movimentar_TipoInvalido_DeveLancarDomainExceptionSemAlterarEstoque()
    {
        var produto = CriarProdutoComEstoque(10);

        Assert.Throws<DomainException>(() =>
            MovimentadorEstoque.Movimentar(produto, (TipoMovimentacaoEstoque)99, Quantidade.Criar(5), Data));

        Assert.Equal(10, produto.QuantidadeEmEstoque.Valor);
    }

    [Fact]
    public void Movimentar_ComProdutoNulo_DeveLancarArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            MovimentadorEstoque.Movimentar(null!, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(1), Data));
    }

    [Fact]
    public void Movimentar_DeveUsarORestauranteDoProduto()
    {
        var restauranteDoProduto = Guid.NewGuid();
        var produto = Produto.Criar(restauranteDoProduto, "Arroz", null, Dinheiro.Criar(10m), Guid.NewGuid(), Data, Quantidade.Criar(5));

        var movimentacao = MovimentadorEstoque.Movimentar(produto, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(1), Data);

        Assert.Equal(restauranteDoProduto, movimentacao.RestauranteId);
    }
}
