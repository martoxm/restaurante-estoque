using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Tests.Integration.Fixtures;

namespace RestauranteEstoque.Tests.Integration.Repositories;

[Collection(nameof(SqlServerCollection))]
public sealed class ProdutoRepositoryTests(SqlServerFixture fixture)
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [DockerFact]
    public async Task Adicionar_DeveGravarELerOProdutoComOsValueObjects()
    {
        if (!fixture.EstaDisponivel)
            return;

        var categoria = DadosTeste.CriarCategoria();
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", "Tipo 1", Dinheiro.Criar(12.34m), categoria, CriadoEm, Quantidade.Criar(7));

        await using (var contexto = fixture.CriarContexto())
        {
            contexto.Categorias.Add(categoria);
            new ProdutoRepository(contexto).Adicionar(produto);
            await contexto.SaveChangesAsync();
        }

        await using var contextoLeitura = fixture.CriarContexto();
        var lido = await new ProdutoRepository(contextoLeitura).ObterPorIdAsync(produto.Id, CancellationToken.None);

        Assert.NotNull(lido);
        Assert.Equal("Arroz", lido.Nome);
        Assert.Equal(Dinheiro.Criar(12.34m), lido.Preco);
        Assert.Equal(Quantidade.Criar(7), lido.QuantidadeEmEstoque);
        Assert.NotNull(lido.Categoria);
        Assert.Equal("Grãos", lido.Categoria.Nome);
    }
}
