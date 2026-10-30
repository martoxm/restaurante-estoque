using RestauranteEstoque.Pagamentos.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Pagamentos.Tests.Common;

namespace RestauranteEstoque.Pagamentos.Tests.Isolamento;

public class PagamentoIsolamentoTests
{
    [Fact]
    public async Task ObterPorId_PagamentoDeOutroRestaurante_DevolveNull()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        var outroRestaurante = Guid.NewGuid();
        var pagamento = TestePagamentos.NovoPagamento(restauranteId: outroRestaurante);

        using (var context = TestePagamentos.CriarContexto(new FakeUsuarioAtualService { RestauranteId = outroRestaurante }, nomeBanco))
        {
            context.Pagamentos.Add(pagamento);
            await context.SaveChangesAsync();
        }

        using var meuContexto = TestePagamentos.CriarContexto(nomeBanco: nomeBanco);
        Assert.Null(await new PagamentoRepository(meuContexto).ObterPorIdAsync(pagamento.Id, CancellationToken.None));
    }

    [Fact]
    public async Task ObterPorChave_ChaveDeOutroRestaurante_DevolveNull()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        var outroRestaurante = Guid.NewGuid();

        using (var context = TestePagamentos.CriarContexto(new FakeUsuarioAtualService { RestauranteId = outroRestaurante }, nomeBanco))
        {
            context.Pagamentos.Add(TestePagamentos.NovoPagamento(restauranteId: outroRestaurante, chave: "chave-igual"));
            await context.SaveChangesAsync();
        }

        using var meuContexto = TestePagamentos.CriarContexto(nomeBanco: nomeBanco);
        Assert.Null(await new PagamentoRepository(meuContexto).ObterPorChaveIdempotenciaAsync("chave-igual", CancellationToken.None));
    }

    [Fact]
    public async Task ListarPendentesDeTodos_IgnoraOFiltroDeRestaurante()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        var semRestaurante = new FakeUsuarioAtualService();

        using (var context = TestePagamentos.CriarContexto(semRestaurante, nomeBanco))
        {
            context.Pagamentos.Add(TestePagamentos.NovoPagamento(restauranteId: Guid.NewGuid()));
            context.Pagamentos.Add(TestePagamentos.NovoPagamento(restauranteId: Guid.NewGuid()));
            await context.SaveChangesAsync();
        }

        using var worker = TestePagamentos.CriarContexto(semRestaurante, nomeBanco);
        var pendentes = await new PagamentoRepository(worker).ListarPendentesDeTodosAsync(10, CancellationToken.None);

        Assert.Equal(2, pendentes.Count);
    }
}
