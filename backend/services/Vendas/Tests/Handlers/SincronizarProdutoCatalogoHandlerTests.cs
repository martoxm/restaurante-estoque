using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Vendas.Application.UseCases.ProdutosCatalogo;
using RestauranteEstoque.Vendas.Domain.Entities;
using RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Vendas.Tests.Common;

namespace RestauranteEstoque.Vendas.Tests.Handlers;

public class SincronizarProdutoCatalogoHandlerTests
{
    private static readonly Guid ProdutoId = Guid.NewGuid();

    private static ProdutoCatalogoMensagem Mensagem(string nome, decimal preco, DateTime ocorridoEm, bool excluido = false) =>
        new(ProdutoId, TesteVendas.RestauranteId, nome, preco, excluido, ocorridoEm);

    private static async Task ProcessarAsync(string nomeBanco, ProdutoCatalogoMensagem mensagem)
    {
        var usuario = new FakeUsuarioAtualService();
        await using var context = TesteVendas.CriarContexto(usuario, nomeBanco);
        var handler = new SincronizarProdutoCatalogoHandler(new ProdutoCatalogoRepository(context), usuario, context);
        await handler.Handle(new SincronizarProdutoCatalogoCommand(mensagem), CancellationToken.None);
    }

    private static async Task<ProdutoCatalogo?> LerAsync(string nomeBanco)
    {
        await using var context = TesteVendas.CriarContexto(nomeBanco: nomeBanco);
        return await new ProdutoCatalogoRepository(context).ObterPorIdAsync(ProdutoId, CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ProdutoNovo_CriaNoCatalogo()
    {
        var banco = Guid.NewGuid().ToString();

        await ProcessarAsync(banco, Mensagem("Coca-Cola", 5m, TesteVendas.Agora));

        var produto = await LerAsync(banco);
        Assert.NotNull(produto);
        Assert.Equal("Coca-Cola", produto.Nome);
        Assert.Equal(5m, produto.Preco.Valor);
        Assert.False(produto.Excluido);
    }

    [Fact]
    public async Task Handle_MudancaMaisNova_AtualizaPreco()
    {
        var banco = Guid.NewGuid().ToString();
        await ProcessarAsync(banco, Mensagem("Coca-Cola", 5m, TesteVendas.Agora));

        await ProcessarAsync(banco, Mensagem("Coca-Cola 2L", 9m, TesteVendas.Agora.AddMinutes(1)));

        var produto = await LerAsync(banco);
        Assert.Equal("Coca-Cola 2L", produto!.Nome);
        Assert.Equal(9m, produto.Preco.Valor);
    }

    [Fact]
    public async Task Handle_MudancaMaisVelha_EhIgnorada()
    {
        var banco = Guid.NewGuid().ToString();
        await ProcessarAsync(banco, Mensagem("Coca-Cola 2L", 9m, TesteVendas.Agora.AddMinutes(1)));

        await ProcessarAsync(banco, Mensagem("Coca-Cola", 5m, TesteVendas.Agora));

        Assert.Equal(9m, (await LerAsync(banco))!.Preco.Valor);
    }

    [Fact]
    public async Task Handle_Excluido_SoMarcaALinha()
    {
        var banco = Guid.NewGuid().ToString();
        await ProcessarAsync(banco, Mensagem("Coca-Cola", 5m, TesteVendas.Agora));

        await ProcessarAsync(banco, Mensagem("Coca-Cola", 5m, TesteVendas.Agora.AddMinutes(1), excluido: true));

        Assert.True((await LerAsync(banco))!.Excluido);
    }

    [Fact]
    public async Task Handle_AlteradoVelhoDepoisDoExcluido_NaoRessuscita()
    {
        var banco = Guid.NewGuid().ToString();
        await ProcessarAsync(banco, Mensagem("Coca-Cola", 5m, TesteVendas.Agora.AddMinutes(1), excluido: true));

        await ProcessarAsync(banco, Mensagem("Coca-Cola", 5m, TesteVendas.Agora));

        Assert.True((await LerAsync(banco))!.Excluido);
    }
}
