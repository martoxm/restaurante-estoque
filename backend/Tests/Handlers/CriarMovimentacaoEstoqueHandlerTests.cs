using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;
using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Tests.Common;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Handlers;

public class CriarMovimentacaoEstoqueHandlerTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_Entrada_DeveAumentarEstoqueERegistrarMovimentacao()
    {
        using var context = TestDbContextFactory.Create();
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria.Id, CriadoEm, quantidadeInicial: Quantidade.Criar(5));
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        var handler = new CriarMovimentacaoEstoqueHandler(
            new FakeMovimentacaoEstoqueRepository(context),
            new FakeProdutoRepository(context),
            context,
            new FakeRelogio());
        var command = new CriarMovimentacaoEstoqueCommand(produto.Id, TipoMovimentacaoEstoque.Entrada, 3, "Compra do mês");

        var movimentacaoId = await handler.Handle(command, CancellationToken.None);

        var produtoAtualizado = await context.Produtos.FindAsync(produto.Id);
        var movimentacaoSalva = await context.MovimentacoesEstoque.FindAsync(movimentacaoId);
        Assert.Equal(8, produtoAtualizado!.QuantidadeEmEstoque.Valor);
        Assert.NotNull(movimentacaoSalva);
        Assert.Equal(TipoMovimentacaoEstoque.Entrada, movimentacaoSalva!.Tipo);
    }

    [Fact]
    public async Task Handle_Saida_ComEstoqueSuficiente_DeveDiminuirEstoque()
    {
        using var context = TestDbContextFactory.Create();
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria.Id, CriadoEm, quantidadeInicial: Quantidade.Criar(5));
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        var handler = new CriarMovimentacaoEstoqueHandler(
            new FakeMovimentacaoEstoqueRepository(context),
            new FakeProdutoRepository(context),
            context,
            new FakeRelogio());
        var command = new CriarMovimentacaoEstoqueCommand(produto.Id, TipoMovimentacaoEstoque.Saida, 3);

        await handler.Handle(command, CancellationToken.None);

        var produtoAtualizado = await context.Produtos.FindAsync(produto.Id);
        Assert.Equal(2, produtoAtualizado!.QuantidadeEmEstoque.Valor);
    }

    [Fact]
    public async Task Handle_Saida_ComEstoqueInsuficiente_DeveLancarDomainException()
    {
        using var context = TestDbContextFactory.Create();
        var categoria = Categoria.Criar(DadosTeste.RestauranteId, "Grãos", CriadoEm);
        var produto = Produto.Criar(DadosTeste.RestauranteId, "Arroz", null, Dinheiro.Criar(10m), categoria.Id, CriadoEm, quantidadeInicial: Quantidade.Criar(5));
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();
        var handler = new CriarMovimentacaoEstoqueHandler(
            new FakeMovimentacaoEstoqueRepository(context),
            new FakeProdutoRepository(context),
            context,
            new FakeRelogio());
        var command = new CriarMovimentacaoEstoqueCommand(produto.Id, TipoMovimentacaoEstoque.Saida, 100);

        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ComProdutoInexistente_DeveLancarNotFoundException()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new CriarMovimentacaoEstoqueHandler(
            new FakeMovimentacaoEstoqueRepository(context),
            new FakeProdutoRepository(context),
            context,
            new FakeRelogio());
        var command = new CriarMovimentacaoEstoqueCommand(Guid.NewGuid(), TipoMovimentacaoEstoque.Entrada, 10);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => handler.Handle(command, CancellationToken.None));
    }
}
