using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class DevolverEstoqueVendaHandlerTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static async Task<(TestDbContext Context, DevolverEstoqueVendaHandler Handler, Produto Arroz)> MontarAsync()
    {
        var usuario = new FakeUsuarioAtualService { RestauranteId = DadosTeste.RestauranteId };
        var context = TestDbContextFactory.Create(usuario);
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var arroz = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm, Quantidade.Criar(15));
        context.Categorias.Add(categoria);
        context.Produtos.Add(arroz);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        usuario.RestauranteId = null;

        var handler = new DevolverEstoqueVendaHandler(
            new FakeProdutoRepository(context),
            new FakeMovimentacaoEstoqueRepository(context),
            new FakeMensagemProcessadaRepository(context),
            usuario,
            context,
            new FakeRelogio(),
            NullLogger<DevolverEstoqueVendaHandler>.Instance);

        return (context, handler, arroz);
    }

    private static DevolverEstoqueVendaCommand Pedido(Guid mensagemId, params ItemBaixaEstoque[] itens) =>
        new(mensagemId, Guid.NewGuid(), DadosTeste.RestauranteId, itens, "Saldo insuficiente.");

    [Fact]
    public async Task Handle_DevolveOsItensComUmaEntrada()
    {
        var (context, handler, arroz) = await MontarAsync();

        await handler.Handle(Pedido(Guid.NewGuid(), new ItemBaixaEstoque(arroz.Id, 5)), CancellationToken.None);

        var produto = await context.Produtos.IgnoreQueryFilters().SingleAsync();
        Assert.Equal(20, produto.QuantidadeEmEstoque.Valor);
        var movimentacao = await context.MovimentacoesEstoque.IgnoreQueryFilters().SingleAsync();
        Assert.Equal(TipoMovimentacaoEstoque.Entrada, movimentacao.Tipo);
        Assert.Contains("Estorno da venda", movimentacao.Observacoes);
    }

    [Fact]
    public async Task Handle_MesmaMensagemDuasVezes_DevolveUmaVezSo()
    {
        var (context, handler, arroz) = await MontarAsync();
        var pedido = Pedido(Guid.NewGuid(), new ItemBaixaEstoque(arroz.Id, 5));

        await handler.Handle(pedido, CancellationToken.None);
        context.ChangeTracker.Clear();
        await handler.Handle(pedido, CancellationToken.None);

        var produto = await context.Produtos.IgnoreQueryFilters().SingleAsync();
        Assert.Equal(20, produto.QuantidadeEmEstoque.Valor);
    }

    [Fact]
    public async Task Handle_ProdutoExcluido_PulaEMarcaComoProcessada()
    {
        var (context, handler, _) = await MontarAsync();

        await handler.Handle(Pedido(Guid.NewGuid(), new ItemBaixaEstoque(Guid.NewGuid(), 3)), CancellationToken.None);

        Assert.Empty(await context.MovimentacoesEstoque.IgnoreQueryFilters().ToListAsync());
        Assert.Equal(1, await context.MensagensProcessadas.CountAsync());
    }
}
