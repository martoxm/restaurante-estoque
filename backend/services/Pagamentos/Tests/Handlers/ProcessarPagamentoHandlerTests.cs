using System.Threading.Channels;
using Microsoft.Extensions.Logging.Abstractions;
using RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;
using RestauranteEstoque.Pagamentos.Domain.Entities;
using RestauranteEstoque.Pagamentos.Domain.Enums;
using RestauranteEstoque.Pagamentos.Infrastructure.Gateways;
using RestauranteEstoque.Pagamentos.Infrastructure.Persistence;
using RestauranteEstoque.Pagamentos.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Pagamentos.Tests.Common;

namespace RestauranteEstoque.Pagamentos.Tests.Handlers;

public class ProcessarPagamentoHandlerTests
{
    private static (ProcessarPagamentoHandler Handler, Channel<CobrancaFalsa> Fila) CriarHandler(AppDbContext context)
    {
        var fila = Channel.CreateUnbounded<CobrancaFalsa>();
        var handler = new ProcessarPagamentoHandler(
            new PagamentoRepository(context), new OutboxMensagemRepository(context), context,
            new GatewayPagamentoFalso(fila), new FakeRelogio(), NullLogger<ProcessarPagamentoHandler>.Instance);
        return (handler, fila);
    }

    private static async Task<Pagamento> SalvarAsync(AppDbContext context, string token)
    {
        var pagamento = TestePagamentos.NovoPagamento(token);
        context.Pagamentos.Add(pagamento);
        await context.SaveChangesAsync();
        return pagamento;
    }

    [Fact]
    public async Task Handle_GatewayAceita_FicaProcessandoESemAvisarAVenda()
    {
        using var context = TestePagamentos.CriarContexto();
        var pagamento = await SalvarAsync(context, GatewayPagamentoFalso.TokenSaldoInsuficiente);
        var (handler, fila) = CriarHandler(context);

        await handler.Handle(new ProcessarPagamentoCommand(pagamento.Id), CancellationToken.None);

        var salvo = await context.Pagamentos.FindAsync(pagamento.Id);
        Assert.Equal(StatusPagamento.Processando, salvo!.Status);
        Assert.Equal($"fake_tr_{pagamento.Id:N}", salvo.IdTransacaoGateway);
        Assert.Empty(context.OutboxMensagens);
        Assert.True(fila.Reader.TryRead(out var cobranca));
        Assert.Equal(pagamento.Id, cobranca.Pedido.PagamentoId);
    }

    [Fact]
    public async Task Handle_GatewayForaCincoVezes_RecusaEAvisaAVenda()
    {
        using var context = TestePagamentos.CriarContexto();
        var pagamento = await SalvarAsync(context, GatewayPagamentoFalso.TokenGatewayFora);
        var (handler, _) = CriarHandler(context);

        for (var i = 1; i < Pagamento.MaximoTentativas; i++)
            await handler.Handle(new ProcessarPagamentoCommand(pagamento.Id), CancellationToken.None);

        Assert.Equal(StatusPagamento.Pendente, (await context.Pagamentos.FindAsync(pagamento.Id))!.Status);
        Assert.Empty(context.OutboxMensagens);

        await handler.Handle(new ProcessarPagamentoCommand(pagamento.Id), CancellationToken.None);

        Assert.Equal(StatusPagamento.Recusado, (await context.Pagamentos.FindAsync(pagamento.Id))!.Status);
        Assert.Single(context.OutboxMensagens);
    }

    [Fact]
    public async Task Handle_PagamentoQueNaoEstaPendente_NaoCobraDeNovo()
    {
        using var context = TestePagamentos.CriarContexto();
        var pagamento = TestePagamentos.NovoPagamento();
        pagamento.MarcarComoProcessando("fake_tr_antigo");
        context.Pagamentos.Add(pagamento);
        await context.SaveChangesAsync();
        var (handler, fila) = CriarHandler(context);

        await handler.Handle(new ProcessarPagamentoCommand(pagamento.Id), CancellationToken.None);

        Assert.False(fila.Reader.TryRead(out _));
    }

    [Theory]
    [InlineData(GatewayPagamentoFalso.TokenAprovado, null)]
    [InlineData(GatewayPagamentoFalso.TokenSaldoInsuficiente, "Saldo insuficiente.")]
    [InlineData(GatewayPagamentoFalso.TokenFraude, "Recusado por suspeita de fraude.")]
    [InlineData("tok_qualquer", "Cartão inválido.")]
    public void Decidir_CadaTokenDeTesteTemSuaResposta(string token, string? motivoEsperado)
    {
        Assert.Equal(motivoEsperado, GatewayPagamentoFalso.Decidir(token));
    }

    [Fact]
    public async Task Handle_GatewayRecusaNaHora_RecusaEAvisaAVenda()
    {
        using var context = TestePagamentos.CriarContexto();
        var pagamento = await SalvarAsync(context, "pm_que_nao_existe");
        var handler = new ProcessarPagamentoHandler(
            new PagamentoRepository(context), new OutboxMensagemRepository(context), context,
            new GatewayQueRecusaNaHora(), new FakeRelogio(), NullLogger<ProcessarPagamentoHandler>.Instance);

        await handler.Handle(new ProcessarPagamentoCommand(pagamento.Id), CancellationToken.None);

        var salvo = await context.Pagamentos.FindAsync(pagamento.Id);
        Assert.Equal(StatusPagamento.Recusado, salvo!.Status);
        Assert.Equal("No such PaymentMethod.", salvo.MotivoRecusa);
        Assert.Single(context.OutboxMensagens);
    }

    private class GatewayQueRecusaNaHora : RestauranteEstoque.Pagamentos.Application.Abstractions.Services.IGatewayPagamento
    {
        public Task<RestauranteEstoque.Pagamentos.Application.Abstractions.Services.CobrancaAceita> CobrarAsync(
            RestauranteEstoque.Pagamentos.Application.Abstractions.Services.PedidoCobranca pedido, CancellationToken cancellationToken) =>
            throw new RestauranteEstoque.Pagamentos.Application.Abstractions.Services.CobrancaRecusadaException("No such PaymentMethod.");
    }
}
