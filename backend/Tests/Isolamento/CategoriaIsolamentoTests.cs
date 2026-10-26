using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Isolamento;

public class CategoriaIsolamentoTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ListarPaginadoAsync_DeveRetornarSoCategoriasDoRestauranteAtual()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        context.Categorias.Add(Categoria.Criar(restauranteA, "Grãos", CriadoEm));
        context.Categorias.Add(Categoria.Criar(restauranteB, "Bebidas", CriadoEm));
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeCategoriaRepository(context);

        var (itens, totalItens) = await repository.ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, direcaoOrdenacao: DirecaoOrdenacao.Asc, CancellationToken.None);

        Assert.Equal(1, totalItens);
        Assert.Equal("Grãos", Assert.Single(itens).Categoria.Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComCategoriaDeOutroRestaurante_DeveRetornarNull()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoriaDoRestauranteB = Categoria.Criar(restauranteB, "Bebidas", CriadoEm);
        context.Categorias.Add(categoriaDoRestauranteB);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeCategoriaRepository(context);

        var categoria = await repository.ObterPorIdAsync(categoriaDoRestauranteB.Id, CancellationToken.None);

        Assert.Null(categoria);
    }
}
