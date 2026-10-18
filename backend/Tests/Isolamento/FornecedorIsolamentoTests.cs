using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Isolamento;

public class FornecedorIsolamentoTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ListarPaginadoAsync_DeveRetornarSoFornecedoresDoRestauranteAtual()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        context.Fornecedores.Add(Fornecedor.Criar(restauranteA, "Distribuidora A", CriadoEm));
        context.Fornecedores.Add(Fornecedor.Criar(restauranteB, "Distribuidora B", CriadoEm));
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeFornecedorRepository(context);

        var (itens, totalItens) = await repository.ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, direcaoOrdenacao: DirecaoOrdenacao.Asc, CancellationToken.None);

        Assert.Equal(1, totalItens);
        Assert.Equal("Distribuidora A", Assert.Single(itens).Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComFornecedorDeOutroRestaurante_DeveRetornarNull()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var fornecedorDoRestauranteB = Fornecedor.Criar(restauranteB, "Distribuidora B", CriadoEm);
        context.Fornecedores.Add(fornecedorDoRestauranteB);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeFornecedorRepository(context);

        var fornecedor = await repository.ObterPorIdAsync(fornecedorDoRestauranteB.Id, CancellationToken.None);

        Assert.Null(fornecedor);
    }
}
