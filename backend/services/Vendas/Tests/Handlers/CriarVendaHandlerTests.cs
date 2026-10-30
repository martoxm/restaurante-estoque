using System.Text.Json;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Vendas.Application.UseCases.Vendas;
using RestauranteEstoque.Vendas.Contracts.Common;
using RestauranteEstoque.Vendas.Contracts.Vendas;
using RestauranteEstoque.Vendas.Domain.Entities;
using RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Vendas.Tests.Common;

namespace RestauranteEstoque.Vendas.Tests.Handlers;

public class CriarVendaHandlerTests
{
    [Fact]
    public async Task Handle_CopiaNomeEPrecoDoCatalogoLocal_EGravaAVenda()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        ProdutoCatalogo coca;
        await using (var preparo = TesteVendas.CriarContexto(nomeBanco: nomeBanco))
            coca = await TesteVendas.AdicionarProdutoAsync(preparo, "Coca-Cola", 5m);

        await using (var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco))
        {
            var handler = new CriarVendaHandler(new VendaRepository(context), new OutboxMensagemRepository(context), new ProdutoCatalogoRepository(context), context, new FakeUsuarioAtualService { RestauranteId = TesteVendas.RestauranteId }, new FakeRelogio());
            await handler.Handle(new CriarVendaCommand([new ItemVendaRequest(coca.Id, 3)], TesteVendas.Token), CancellationToken.None);
        }

        await using var leitura = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        var (vendas, _) = await new VendaRepository(leitura).ListarPaginadoAsync(1, 10, DirecaoOrdenacao.Desc, CancellationToken.None);
        var venda = Assert.Single(vendas);
        var item = Assert.Single(venda.Itens);
        Assert.Equal("Coca-Cola", item.NomeProduto);
        Assert.Equal(5m, item.PrecoUnitario.Valor);
        Assert.Equal(15m, venda.Total.Valor);
        Assert.Equal(TesteVendas.Agora, venda.CriadoEm);
    }

    [Fact]
    public async Task Handle_VendaValida_GravaPedidoDeBaixaNoOutbox()
    {
        var nomeBanco = Guid.NewGuid().ToString();
        ProdutoCatalogo coca;
        await using (var preparo = TesteVendas.CriarContexto(nomeBanco: nomeBanco))
            coca = await TesteVendas.AdicionarProdutoAsync(preparo, "Coca-Cola", 5m);
        Guid vendaId;

        await using (var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco))
        {
            var handler = new CriarVendaHandler(new VendaRepository(context), new OutboxMensagemRepository(context), new ProdutoCatalogoRepository(context), context, new FakeUsuarioAtualService { RestauranteId = TesteVendas.RestauranteId }, new FakeRelogio());
            vendaId = await handler.Handle(new CriarVendaCommand([new ItemVendaRequest(coca.Id, 3)], TesteVendas.Token), CancellationToken.None);
        }

        await using var leitura = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        var pendentes = await new OutboxMensagemRepository(leitura).ListarPendentesAsync(10, TesteVendas.Agora, CancellationToken.None);
        var linha = Assert.Single(pendentes);
        Assert.Equal(Filas.BaixarEstoque, linha.Fila);
        Assert.Equal(nameof(BaixarEstoqueMensagem), linha.Tipo);

        var pedido = JsonSerializer.Deserialize<BaixarEstoqueMensagem>(linha.Conteudo)!;
        Assert.Equal(vendaId, pedido.VendaId);
        Assert.Equal(TesteVendas.RestauranteId, pedido.RestauranteId);
        var item = Assert.Single(pedido.Itens);
        Assert.Equal(coca.Id, item.ProdutoId);
        Assert.Equal(3, item.Quantidade);
    }

    [Fact]
    public async Task Handle_ProdutoForaDoCatalogo_LancaNaoEncontrado()
    {
        await using var context = TesteVendas.CriarContexto();
        var handler = new CriarVendaHandler(new VendaRepository(context), new OutboxMensagemRepository(context), new ProdutoCatalogoRepository(context), context, new FakeUsuarioAtualService { RestauranteId = TesteVendas.RestauranteId }, new FakeRelogio());

        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            handler.Handle(new CriarVendaCommand([new ItemVendaRequest(Guid.NewGuid(), 1)], TesteVendas.Token), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TokenSemRestaurante_LancaSessaoInvalida()
    {
        await using var context = TesteVendas.CriarContexto();
        var handler = new CriarVendaHandler(new VendaRepository(context), new OutboxMensagemRepository(context), new ProdutoCatalogoRepository(context), context, new FakeUsuarioAtualService(), new FakeRelogio());

        await Assert.ThrowsAsync<SessaoInvalidaException>(() =>
            handler.Handle(new CriarVendaCommand([new ItemVendaRequest(Guid.NewGuid(), 1)], TesteVendas.Token), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ProdutoExcluidoNoCatalogo_LancaNaoEncontrado()
    {
        await using var context = TesteVendas.CriarContexto();
        var produto = await TesteVendas.AdicionarProdutoAsync(context, "Coca-Cola", 5m, excluido: true);
        var handler = new CriarVendaHandler(new VendaRepository(context), new OutboxMensagemRepository(context), new ProdutoCatalogoRepository(context), context, new FakeUsuarioAtualService { RestauranteId = TesteVendas.RestauranteId }, new FakeRelogio());

        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            handler.Handle(new CriarVendaCommand([new ItemVendaRequest(produto.Id, 1)], TesteVendas.Token), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ProdutoDeOutroRestaurante_LancaNaoEncontrado()
    {
        await using var context = TesteVendas.CriarContexto();
        var produto = await TesteVendas.AdicionarProdutoAsync(context, "Coca-Cola", 5m, restauranteId: Guid.NewGuid());
        var handler = new CriarVendaHandler(new VendaRepository(context), new OutboxMensagemRepository(context), new ProdutoCatalogoRepository(context), context, new FakeUsuarioAtualService { RestauranteId = TesteVendas.RestauranteId }, new FakeRelogio());

        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            handler.Handle(new CriarVendaCommand([new ItemVendaRequest(produto.Id, 1)], TesteVendas.Token), CancellationToken.None));
    }
}
