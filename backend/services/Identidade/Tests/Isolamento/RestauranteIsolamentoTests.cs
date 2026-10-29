using RestauranteEstoque.Identidade.Contracts.Common;
using RestauranteEstoque.Identidade.Domain.Entities;
using RestauranteEstoque.Identidade.Tests.Common;

namespace RestauranteEstoque.Identidade.Tests.Isolamento;

public class RestauranteIsolamentoTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ListarPaginadoAsync_DeveRetornarSoORestauranteAtual()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Restaurante.Criar("Restaurante A", CriadoEm);
        var restauranteB = Restaurante.Criar("Restaurante B", CriadoEm);
        context.Restaurantes.AddRange(restauranteA, restauranteB);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA.Id;
        var repository = new FakeRestauranteRepository(context);

        var (itens, totalItens) = await repository.ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, direcaoOrdenacao: DirecaoOrdenacao.Asc, CancellationToken.None);

        Assert.Equal(1, totalItens);
        Assert.Equal("Restaurante A", Assert.Single(itens).Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdDeOutroRestaurante_DeveRetornarNull()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Restaurante.Criar("Restaurante A", CriadoEm);
        var restauranteB = Restaurante.Criar("Restaurante B", CriadoEm);
        context.Restaurantes.AddRange(restauranteA, restauranteB);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA.Id;
        var repository = new FakeRestauranteRepository(context);

        var restaurante = await repository.ObterPorIdAsync(restauranteB.Id, CancellationToken.None);

        Assert.Null(restaurante);
    }
}
