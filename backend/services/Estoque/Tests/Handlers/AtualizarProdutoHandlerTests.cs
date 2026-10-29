using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Application.UseCases.Produtos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class AtualizarProdutoHandlerTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_ComCategoriaDoMesmoRestaurante_DeveTrocarACategoria()
    {
        using var context = TestDbContextFactory.Create();
        var (produto, _) = await CriarProdutoAsync(context);
        var categoriaNova = Categoria.Criar(DadosTeste.RestauranteId, "Bebidas", CriadoEm);
        context.Categorias.Add(categoriaNova);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = CriarHandler(context);

        await handler.Handle(new AtualizarProdutoCommand(produto.Id, "Arroz", null, 12m, categoriaNova.Id), CancellationToken.None);

        context.ChangeTracker.Clear();
        var produtoSalvo = await context.Produtos.FindAsync(produto.Id);
        Assert.Equal(categoriaNova.Id, produtoSalvo!.CategoriaId);
        Assert.Equal(12m, produtoSalvo.Preco.Valor);
    }

    [Fact]
    public async Task Handle_ComCategoriaDeOutroRestaurante_DeveLancarNaoEncontradoExceptionENaoTrocar()
    {
        using var context = TestDbContextFactory.Create();
        var (produto, categoriaOriginal) = await CriarProdutoAsync(context);
        var categoriaDeOutroRestaurante = Categoria.Criar(Guid.NewGuid(), "Bebidas", CriadoEm);
        context.Categorias.Add(categoriaDeOutroRestaurante);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = CriarHandler(context);

        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            handler.Handle(new AtualizarProdutoCommand(produto.Id, "Arroz", null, 12m, categoriaDeOutroRestaurante.Id), CancellationToken.None));

        var produtoSalvo = await context.Produtos.FindAsync(produto.Id);
        Assert.Equal(categoriaOriginal.Id, produtoSalvo!.CategoriaId);
    }

    private static AtualizarProdutoHandler CriarHandler(TestDbContext context) =>
        new(new FakeProdutoRepository(context), new FakeCategoriaRepository(context), context);

    private static async Task<(Produto Produto, Categoria Categoria)> CriarProdutoAsync(TestDbContext context)
    {
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm);
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        return (produto, categoria);
    }
}
