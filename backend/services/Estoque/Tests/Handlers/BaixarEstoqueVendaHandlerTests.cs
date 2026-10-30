using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class BaixarEstoqueVendaHandlerTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private sealed record Cenario(TestDbContext Context, FakeOutboxMensagemRepository Outbox, BaixarEstoqueVendaHandler Handler, Produto Arroz, Produto Feijao);

    private static async Task<Cenario> MontarAsync()
    {
        var usuario = new FakeUsuarioAtualService { RestauranteId = DadosTeste.RestauranteId };
        var context = TestDbContextFactory.Create(usuario);
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var arroz = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm, Quantidade.Criar(20));
        var feijao = Produto.Criar(DadosTeste.RestauranteId, "Feijão", null, Dinheiro.Criar(8m), categoria, CriadoEm, Quantidade.Criar(2));
        context.Categorias.Add(categoria);
        context.Produtos.AddRange(arroz, feijao);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        usuario.RestauranteId = null;

        var outbox = new FakeOutboxMensagemRepository();
        var handler = new BaixarEstoqueVendaHandler(
            new FakeProdutoRepository(context),
            new FakeMovimentacaoEstoqueRepository(context),
            outbox,
            new FakeMensagemProcessadaRepository(context),
            usuario,
            context,
            new FakeRelogio());

        return new Cenario(context, outbox, handler, arroz, feijao);
    }

    private static BaixarEstoqueVendaCommand Pedido(Guid mensagemId, params ItemBaixaEstoque[] itens) =>
        new(mensagemId, Guid.NewGuid(), DadosTeste.RestauranteId, itens);

    private static ResultadoBaixaEstoqueMensagem Resposta(FakeOutboxMensagemRepository outbox)
    {
        var linha = Assert.Single(outbox.Mensagens);
        Assert.Equal(Filas.ResultadoBaixaEstoque, linha.Fila);
        return JsonSerializer.Deserialize<ResultadoBaixaEstoqueMensagem>(linha.Conteudo)!;
    }

    [Fact]
    public async Task Handle_ComSaldo_BaixaTodosOsItensERespondeBaixado()
    {
        var c = await MontarAsync();

        await c.Handler.Handle(Pedido(Guid.NewGuid(), new ItemBaixaEstoque(c.Arroz.Id, 5), new ItemBaixaEstoque(c.Feijao.Id, 2)), CancellationToken.None);

        var produtos = await c.Context.Produtos.IgnoreQueryFilters().ToListAsync();
        Assert.Equal(15, produtos.Single(p => p.Id == c.Arroz.Id).QuantidadeEmEstoque.Valor);
        Assert.Equal(0, produtos.Single(p => p.Id == c.Feijao.Id).QuantidadeEmEstoque.Valor);
        Assert.Equal(2, await c.Context.MovimentacoesEstoque.IgnoreQueryFilters().CountAsync());
        Assert.True(Resposta(c.Outbox).Baixado);
    }

    [Fact]
    public async Task Handle_UmItemSemSaldo_NaoBaixaNadaERespondeSemSaldo()
    {
        var c = await MontarAsync();

        await c.Handler.Handle(Pedido(Guid.NewGuid(), new ItemBaixaEstoque(c.Arroz.Id, 5), new ItemBaixaEstoque(c.Feijao.Id, 3)), CancellationToken.None);

        var arroz = await c.Context.Produtos.IgnoreQueryFilters().SingleAsync(p => p.Id == c.Arroz.Id);
        Assert.Equal(20, arroz.QuantidadeEmEstoque.Valor);
        Assert.Equal(0, await c.Context.MovimentacoesEstoque.IgnoreQueryFilters().CountAsync());
        var resposta = Resposta(c.Outbox);
        Assert.False(resposta.Baixado);
        Assert.Contains("Feijão", resposta.Motivo);
    }

    [Fact]
    public async Task Handle_MesmaMensagemDuasVezes_BaixaUmaVezSo()
    {
        var c = await MontarAsync();
        var mensagemId = Guid.NewGuid();

        await c.Handler.Handle(Pedido(mensagemId, new ItemBaixaEstoque(c.Arroz.Id, 5)), CancellationToken.None);
        await c.Handler.Handle(Pedido(mensagemId, new ItemBaixaEstoque(c.Arroz.Id, 5)), CancellationToken.None);

        var arroz = await c.Context.Produtos.IgnoreQueryFilters().SingleAsync(p => p.Id == c.Arroz.Id);
        Assert.Equal(15, arroz.QuantidadeEmEstoque.Valor);
        Assert.Single(c.Outbox.Mensagens);
        Assert.Equal(1, await c.Context.MensagensProcessadas.CountAsync());
    }

    [Fact]
    public async Task Handle_ProdutoDeOutroRestaurante_RespondeSemSaldo()
    {
        var c = await MontarAsync();

        var pedido = new BaixarEstoqueVendaCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), [new(c.Arroz.Id, 1)]);
        await c.Handler.Handle(pedido, CancellationToken.None);

        var arroz = await c.Context.Produtos.IgnoreQueryFilters().SingleAsync(p => p.Id == c.Arroz.Id);
        Assert.Equal(20, arroz.QuantidadeEmEstoque.Valor);
        Assert.False(Resposta(c.Outbox).Baixado);
    }
}
