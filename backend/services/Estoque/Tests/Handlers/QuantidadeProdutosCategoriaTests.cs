using RestauranteEstoque.Application.UseCases.Categorias;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class QuantidadeProdutosCategoriaTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ListarCategorias_DeveTrazerAQuantidadeDeProdutosDeCadaCategoria()
    {
        using var context = TestDbContextFactory.Create();
        var graos = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var bebidas = Categoria.Criar(DadosTeste.RestauranteId, "Bebidas", CriadoEm);
        context.Categorias.AddRange(graos, bebidas);
        context.Produtos.Add(Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), graos, CriadoEm));
        context.Produtos.Add(Produto.Criar(DadosTeste.RestauranteId, "Feijão", null, Dinheiro.Criar(8m), graos, CriadoEm));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = new ListarCategoriasHandler(new FakeCategoriaRepository(context));

        var resultado = await handler.Handle(
            new ListarCategoriasQuery(1, 10, null, DirecaoOrdenacao.Asc), CancellationToken.None);

        Assert.Equal(0, resultado.Itens.Single(c => c.Nome == "Bebidas").QuantidadeProdutos);
        Assert.Equal(2, resultado.Itens.Single(c => c.Nome == "Grãos").QuantidadeProdutos);
    }

    [Fact]
    public async Task ObterCategoriaPorId_DeveTrazerAQuantidadeDeProdutos()
    {
        using var context = TestDbContextFactory.Create();
        var graos = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        context.Categorias.Add(graos);
        context.Produtos.Add(Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), graos, CriadoEm));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var handler = new ObterCategoriaPorIdHandler(new FakeCategoriaRepository(context), new FakeProdutoRepository(context));

        var categoria = await handler.Handle(new ObterCategoriaPorIdQuery(graos.Id), CancellationToken.None);

        Assert.NotNull(categoria);
        Assert.Equal(1, categoria.QuantidadeProdutos);
    }
}
