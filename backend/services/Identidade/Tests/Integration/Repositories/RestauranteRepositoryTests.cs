using RestauranteEstoque.Identidade.Contracts.Common;
using RestauranteEstoque.Identidade.Domain.Entities;
using RestauranteEstoque.Identidade.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Identidade.Tests.Common;
using RestauranteEstoque.Identidade.Tests.Integration.Fixtures;

namespace RestauranteEstoque.Identidade.Tests.Integration.Repositories;

[Collection(nameof(SqlServerCollection))]
public sealed class RestauranteRepositoryTests(SqlServerFixture fixture)
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [DockerFact]
    public async Task Restaurante_DeveEnxergarSoOProprio()
    {
        if (!fixture.EstaDisponivel)
            return;

        var restauranteA = Restaurante.Criar("Restaurante A", CriadoEm);
        var restauranteB = Restaurante.Criar("Restaurante B", CriadoEm);
        await using (var escrita = fixture.CriarContexto())
        {
            escrita.Restaurantes.AddRange(restauranteA, restauranteB);
            await escrita.SaveChangesAsync();
        }

        await using var contexto = fixture.CriarContexto(new FakeUsuarioAtualService { RestauranteId = restauranteA.Id });
        var repository = new RestauranteRepository(contexto);
        var (itens, totalItens) = await repository.ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, DirecaoOrdenacao.Asc, CancellationToken.None);
        var outro = await repository.ObterPorIdAsync(restauranteB.Id, CancellationToken.None);

        Assert.Equal(1, totalItens);
        Assert.Equal(restauranteA.Id, Assert.Single(itens).Id);
        Assert.Null(outro);
    }
}
