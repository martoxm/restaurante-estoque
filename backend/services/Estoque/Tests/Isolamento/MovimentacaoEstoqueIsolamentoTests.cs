using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Tests.Common;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Isolamento;

public class MovimentacaoEstoqueIsolamentoTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ExisteParaProdutoAsync_ComMovimentacaoDeOutroRestaurante_DeveRetornarFalse()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoria = Categoria.Criar(restauranteB, "Grãos", CriadoEm);
        var produto = Produto.Criar(restauranteB, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm);
        var movimentacao = MovimentacaoEstoque.Criar(restauranteB, produto.Id, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(5), CriadoEm);
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        context.MovimentacoesEstoque.Add(movimentacao);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeMovimentacaoEstoqueRepository(context);

        var existe = await repository.ExisteParaProdutoAsync(produto.Id, CancellationToken.None);

        Assert.False(existe);
    }

    [Fact]
    public async Task ExisteParaProdutoAsync_ComMovimentacaoDoProprioRestaurante_DeveRetornarTrue()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var categoria = Categoria.Criar(restauranteA, "Grãos", CriadoEm);
        var produto = Produto.Criar(restauranteA, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm);
        var movimentacao = MovimentacaoEstoque.Criar(restauranteA, produto.Id, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(5), CriadoEm);
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        context.MovimentacoesEstoque.Add(movimentacao);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeMovimentacaoEstoqueRepository(context);

        var existe = await repository.ExisteParaProdutoAsync(produto.Id, CancellationToken.None);

        Assert.True(existe);
    }
}
