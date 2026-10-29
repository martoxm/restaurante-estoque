using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Application.UseCases.Produtos;
using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class ExcluirProdutoHandlerTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_ProdutoSemMovimentacoes_DeveExcluir()
    {
        using var context = TestDbContextFactory.Create();
        var produto = await CriarProdutoAsync(context);

        var handler = CriarHandler(context);

        await handler.Handle(new ExcluirProdutoCommand(produto.Id), CancellationToken.None);

        Assert.False(await context.Produtos.AnyAsync(p => p.Id == produto.Id));
    }

    [Fact]
    public async Task Handle_ProdutoComMovimentacoes_DeveLancarDomainExceptionENaoExcluir()
    {
        using var context = TestDbContextFactory.Create();
        var produto = await CriarProdutoAsync(context);
        context.MovimentacoesEstoque.Add(MovimentacaoEstoque.Criar(
            DadosTeste.RestauranteId, produto.Id, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(5), CriadoEm));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = CriarHandler(context);

        var excecao = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(new ExcluirProdutoCommand(produto.Id), CancellationToken.None));

        Assert.Equal("Não é possível excluir um produto que possui movimentações de estoque vinculadas.", excecao.Message);
        Assert.True(await context.Produtos.AnyAsync(p => p.Id == produto.Id));
    }

    [Fact]
    public async Task Handle_ProdutoInexistente_DeveLancarNaoEncontradoException()
    {
        using var context = TestDbContextFactory.Create();
        var handler = CriarHandler(context);

        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            handler.Handle(new ExcluirProdutoCommand(Guid.NewGuid()), CancellationToken.None));
    }

    private static ExcluirProdutoHandler CriarHandler(TestDbContext context) =>
        new(new FakeProdutoRepository(context), new FakeMovimentacaoEstoqueRepository(context), context);

    private static async Task<Produto> CriarProdutoAsync(TestDbContext context)
    {
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm);
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        return produto;
    }
}
