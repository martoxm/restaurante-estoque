using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Vendas.Application.UseCases.Vendas;
using RestauranteEstoque.Vendas.Domain.Entities;
using RestauranteEstoque.Vendas.Domain.Enums;
using RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Vendas.Tests.Common;

namespace RestauranteEstoque.Vendas.Tests.Handlers;

public class ProcessarResultadoBaixaHandlerTests
{
    private static async Task<(string NomeBanco, Guid VendaId)> CriarVendaPendenteAsync()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        await using var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        var venda = Venda.Criar(TesteVendas.RestauranteId, TesteVendas.Agora,
            [new NovoItemVenda(Guid.NewGuid(), "Coca-Cola", Dinheiro.Criar(5m), Quantidade.Criar(2))], TesteVendas.Token);
        context.Vendas.Add(venda);
        await context.SaveChangesAsync();
        return (nomeBanco, venda.Id);
    }

    private static async Task ProcessarAsync(string nomeBanco, Guid mensagemId, ResultadoBaixaEstoqueMensagem resultado)
    {
        var usuario = new FakeUsuarioAtualService();
        await using var context = TesteVendas.CriarContexto(usuario, nomeBanco);
        var handler = new ProcessarResultadoBaixaHandler(
            new VendaRepository(context),
            new MensagemProcessadaRepository(context),
            new OutboxMensagemRepository(context),
            usuario,
            context,
            new FakeRelogio(),
            NullLogger<ProcessarResultadoBaixaHandler>.Instance);
        await handler.Handle(new ProcessarResultadoBaixaCommand(mensagemId, resultado), CancellationToken.None);
    }

    private static async Task<StatusVenda> StatusAsync(string nomeBanco, Guid vendaId)
    {
        await using var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        return (await context.Vendas.SingleAsync(venda => venda.Id == vendaId)).Status;
    }

    [Fact]
    public async Task Handle_Baixado_FicaAguardandoPagamentoEPedeACobranca()
    {
        var (nomeBanco, vendaId) = await CriarVendaPendenteAsync();

        await ProcessarAsync(nomeBanco, Guid.NewGuid(), new(vendaId, TesteVendas.RestauranteId, true, null));

        Assert.Equal(StatusVenda.AguardandoPagamento, await StatusAsync(nomeBanco, vendaId));
        await using var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        var pedido = await context.OutboxMensagens.SingleAsync();
        Assert.Equal(Filas.ProcessarPagamento, pedido.Fila);
        Assert.Contains("\"Valor\":10", pedido.Conteudo);
    }

    [Fact]
    public async Task Handle_SemSaldo_CancelaAVenda()
    {
        var (nomeBanco, vendaId) = await CriarVendaPendenteAsync();

        await ProcessarAsync(nomeBanco, Guid.NewGuid(), new(vendaId, TesteVendas.RestauranteId, false, "Estoque insuficiente."));

        Assert.Equal(StatusVenda.Cancelada, await StatusAsync(nomeBanco, vendaId));
    }

    [Fact]
    public async Task Handle_MesmaMensagemDuasVezes_ProcessaUmaVezSo()
    {
        var (nomeBanco, vendaId) = await CriarVendaPendenteAsync();
        var mensagemId = Guid.NewGuid();
        var resultado = new ResultadoBaixaEstoqueMensagem(vendaId, TesteVendas.RestauranteId, true, null);

        await ProcessarAsync(nomeBanco, mensagemId, resultado);
        await ProcessarAsync(nomeBanco, mensagemId, resultado);

        Assert.Equal(StatusVenda.AguardandoPagamento, await StatusAsync(nomeBanco, vendaId));
        await using var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        Assert.Equal(1, await context.MensagensProcessadas.CountAsync());
    }

    [Fact]
    public async Task Handle_VendaDeOutroRestaurante_NaoMexeEMarcaComoProcessada()
    {
        var (nomeBanco, vendaId) = await CriarVendaPendenteAsync();

        await ProcessarAsync(nomeBanco, Guid.NewGuid(), new(vendaId, Guid.NewGuid(), true, null));

        Assert.Equal(StatusVenda.Pendente, await StatusAsync(nomeBanco, vendaId));
        await using var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        Assert.Equal(1, await context.MensagensProcessadas.CountAsync());
    }
}
