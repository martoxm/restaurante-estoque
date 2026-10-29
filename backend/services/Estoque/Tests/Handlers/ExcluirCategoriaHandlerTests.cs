using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Application.UseCases.Categorias;
using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class ExcluirCategoriaHandlerTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_CategoriaSemProdutos_DeveExcluir()
    {
        using var context = TestDbContextFactory.Create();
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Bebidas", CriadoEm);
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();

        var handler = new ExcluirCategoriaHandler(new FakeCategoriaRepository(context), new FakeProdutoRepository(context), context);

        await handler.Handle(new ExcluirCategoriaCommand(categoria.Id), CancellationToken.None);

        Assert.False(await context.Categorias.AnyAsync(c => c.Id == categoria.Id));
    }

    [Fact]
    public async Task Handle_CategoriaComProdutos_DeveLancarDomainExceptionENaoExcluir()
    {
        using var context = TestDbContextFactory.Create();
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        context.Categorias.Add(categoria);
        context.Produtos.Add(Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = new ExcluirCategoriaHandler(new FakeCategoriaRepository(context), new FakeProdutoRepository(context), context);

        var excecao = await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(new ExcluirCategoriaCommand(categoria.Id), CancellationToken.None));

        Assert.Equal("Não é possível excluir uma categoria que possui produtos vinculados.", excecao.Message);
        Assert.True(await context.Categorias.AnyAsync(c => c.Id == categoria.Id));
    }

    [Fact]
    public async Task Handle_CategoriaInexistente_DeveLancarNaoEncontradoException()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new ExcluirCategoriaHandler(new FakeCategoriaRepository(context), new FakeProdutoRepository(context), context);

        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            handler.Handle(new ExcluirCategoriaCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
