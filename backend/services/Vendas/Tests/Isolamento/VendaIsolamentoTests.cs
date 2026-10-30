using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Vendas.Contracts.Common;
using RestauranteEstoque.Vendas.Domain.Entities;
using RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Vendas.Tests.Common;

namespace RestauranteEstoque.Vendas.Tests.Isolamento;

public class VendaIsolamentoTests
{
    private static readonly Guid RestauranteA = Guid.NewGuid();
    private static readonly Guid RestauranteB = Guid.NewGuid();

    private static async Task<(string NomeBanco, Venda VendaA, Venda VendaB)> CriarCenarioAsync()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        var vendaA = Venda.Criar(RestauranteA, TesteVendas.Agora, [new NovoItemVenda(Guid.NewGuid(), "X-Burger", Dinheiro.Criar(20m), Quantidade.Criar(1))], TesteVendas.Token);
        var vendaB = Venda.Criar(RestauranteB, TesteVendas.Agora, [new NovoItemVenda(Guid.NewGuid(), "Big Mac", Dinheiro.Criar(25m), Quantidade.Criar(1))], TesteVendas.Token);

        await using var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        context.Vendas.AddRange(vendaA, vendaB);
        await context.SaveChangesAsync();

        return (nomeBanco, vendaA, vendaB);
    }

    [Fact]
    public async Task ListarPaginado_SoTrazAsVendasDoRestauranteLogado()
    {
        var (nomeBanco, vendaA, _) = await CriarCenarioAsync();

        await using var context = TesteVendas.CriarContexto(new FakeUsuarioAtualService { RestauranteId = RestauranteA }, nomeBanco);
        var (itens, total) = await new VendaRepository(context).ListarPaginadoAsync(1, 10, DirecaoOrdenacao.Desc, CancellationToken.None);

        Assert.Equal(1, total);
        Assert.Equal(vendaA.Id, Assert.Single(itens).Id);
    }

    [Fact]
    public async Task ObterPorId_DeVendaDeOutroRestaurante_DevolveNull()
    {
        var (nomeBanco, _, vendaB) = await CriarCenarioAsync();

        await using var context = TesteVendas.CriarContexto(new FakeUsuarioAtualService { RestauranteId = RestauranteA }, nomeBanco);

        Assert.Null(await new VendaRepository(context).ObterPorIdAsync(vendaB.Id, CancellationToken.None));
    }
}
