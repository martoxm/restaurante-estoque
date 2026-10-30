using Microsoft.Extensions.Logging.Abstractions;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;
using RestauranteEstoque.Pagamentos.Domain.Entities;
using RestauranteEstoque.Pagamentos.Domain.Enums;
using RestauranteEstoque.Pagamentos.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Pagamentos.Tests.Common;

namespace RestauranteEstoque.Pagamentos.Tests.Handlers;

public class ProcessarWebhookGatewayHandlerTests
{
    private static async Task<(string NomeBanco, Pagamento Pagamento)> CriarProcessandoAsync()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        using var context = TestePagamentos.CriarContexto(nomeBanco: nomeBanco);
        var pagamento = TestePagamentos.NovoPagamento();
        pagamento.MarcarComoProcessando("fake_tr_1");
        context.Pagamentos.Add(pagamento);
        await context.SaveChangesAsync();
        return (nomeBanco, pagamento);
    }

    private static async Task ProcessarAsync(string nomeBanco, ProcessarWebhookGatewayCommand command)
    {
        var semRestaurante = new FakeUsuarioAtualService();
        using var context = TestePagamentos.CriarContexto(semRestaurante, nomeBanco);
        var handler = new ProcessarWebhookGatewayHandler(
            new PagamentoRepository(context), new OutboxMensagemRepository(context), semRestaurante,
            context, new FakeRelogio(), NullLogger<ProcessarWebhookGatewayHandler>.Instance);
        await handler.Handle(command, CancellationToken.None);
    }

    private static (StatusPagamento Status, int Mensagens, string? Fila) Ler(string nomeBanco)
    {
        using var context = TestePagamentos.CriarContexto(nomeBanco: nomeBanco);
        var mensagens = context.OutboxMensagens.ToList();
        return (context.Pagamentos.Single().Status, mensagens.Count, mensagens.FirstOrDefault()?.Fila);
    }

    [Fact]
    public async Task Handle_Aprovada_AprovaEAvisaAVenda()
    {
        var (nomeBanco, pagamento) = await CriarProcessandoAsync();

        await ProcessarAsync(nomeBanco, new(pagamento.Id, pagamento.RestauranteId, "fake_tr_1", true, null));

        var (status, mensagens, fila) = Ler(nomeBanco);
        Assert.Equal(StatusPagamento.Aprovado, status);
        Assert.Equal(1, mensagens);
        Assert.Equal(Filas.ResultadoPagamento, fila);
    }

    [Fact]
    public async Task Handle_Recusada_RecusaComOMotivo()
    {
        var (nomeBanco, pagamento) = await CriarProcessandoAsync();

        await ProcessarAsync(nomeBanco, new(pagamento.Id, pagamento.RestauranteId, "fake_tr_1", false, "Saldo insuficiente."));

        using var context = TestePagamentos.CriarContexto(nomeBanco: nomeBanco);
        var salvo = context.Pagamentos.Single();
        Assert.Equal(StatusPagamento.Recusado, salvo.Status);
        Assert.Equal("Saldo insuficiente.", salvo.MotivoRecusa);
    }

    [Fact]
    public async Task Handle_MesmoAvisoDuasVezes_AvisaAVendaUmaVezSo()
    {
        var (nomeBanco, pagamento) = await CriarProcessandoAsync();
        var aviso = new ProcessarWebhookGatewayCommand(pagamento.Id, pagamento.RestauranteId, "fake_tr_1", true, null);

        await ProcessarAsync(nomeBanco, aviso);
        await ProcessarAsync(nomeBanco, aviso);

        Assert.Equal(1, Ler(nomeBanco).Mensagens);
    }

    [Fact]
    public async Task Handle_TransacaoDiferente_Ignora()
    {
        var (nomeBanco, pagamento) = await CriarProcessandoAsync();

        await ProcessarAsync(nomeBanco, new(pagamento.Id, pagamento.RestauranteId, "fake_tr_outra", true, null));

        var (status, mensagens, _) = Ler(nomeBanco);
        Assert.Equal(StatusPagamento.Processando, status);
        Assert.Equal(0, mensagens);
    }
}
