using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Produtos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Isolamento;

public class ProdutoIsolamentoTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ListarPaginadoAsync_DeveRetornarSoProdutosDoRestauranteAtual()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoriaDoA = Categoria.Criar(restauranteA, "Grãos", CriadoEm);
        var categoriaDoB = Categoria.Criar(restauranteB, "Grãos", CriadoEm);
        context.Categorias.AddRange(categoriaDoA, categoriaDoB);
        context.Produtos.Add(Produto.Criar(restauranteA, "Arroz", null, Dinheiro.Criar(10m), categoriaDoA, CriadoEm));
        context.Produtos.Add(Produto.Criar(restauranteB, "Feijão", null, Dinheiro.Criar(8m), categoriaDoB, CriadoEm));
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeProdutoRepository(context);

        var (itens, totalItens) = await repository.ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, categoriaId: null,
            ordenarPor: OrdenacaoProduto.Nome, direcaoOrdenacao: DirecaoOrdenacao.Asc, CancellationToken.None);

        Assert.Equal(1, totalItens);
        Assert.Equal("Arroz", Assert.Single(itens).Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComProdutoDeOutroRestaurante_DeveRetornarNull()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoria = Categoria.Criar(restauranteB, "Grãos", CriadoEm);
        var produtoDoRestauranteB = Produto.Criar(restauranteB, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm);
        context.Categorias.Add(categoria);
        context.Produtos.Add(produtoDoRestauranteB);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeProdutoRepository(context);

        var produto = await repository.ObterPorIdAsync(produtoDoRestauranteB.Id, CancellationToken.None);

        Assert.Null(produto);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComProdutoDoProprioRestaurante_DeveRetornarOProduto()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var categoria = Categoria.Criar(restauranteA, "Grãos", CriadoEm);
        var produto = Produto.Criar(restauranteA, "Arroz", null, Dinheiro.Criar(10m), categoria, CriadoEm);
        context.Categorias.Add(categoria);
        context.Produtos.Add(produto);
        await context.SaveChangesAsync();

        usuarioAtualService.RestauranteId = restauranteA;
        var repository = new FakeProdutoRepository(context);

        var produtoEncontrado = await repository.ObterPorIdAsync(produto.Id, CancellationToken.None);

        Assert.NotNull(produtoEncontrado);
    }

    [Fact]
    public async Task ListarPaginadoAsync_TrocandoORestauranteLogado_DeveRefletirNaProximaConsulta()
    {
        var usuarioAtualService = new FakeUsuarioAtualService();
        using var context = TestDbContextFactory.Create(usuarioAtualService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoriaDoA = Categoria.Criar(restauranteA, "Grãos", CriadoEm);
        var categoriaDoB = Categoria.Criar(restauranteB, "Grãos", CriadoEm);
        context.Categorias.AddRange(categoriaDoA, categoriaDoB);
        context.Produtos.Add(Produto.Criar(restauranteA, "Arroz", null, Dinheiro.Criar(10m), categoriaDoA, CriadoEm));
        context.Produtos.Add(Produto.Criar(restauranteB, "Feijão", null, Dinheiro.Criar(8m), categoriaDoB, CriadoEm));
        await context.SaveChangesAsync();
        var repository = new FakeProdutoRepository(context);

        usuarioAtualService.RestauranteId = restauranteA;
        var (itensDoA, _) = await repository.ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, categoriaId: null,
            ordenarPor: OrdenacaoProduto.Nome, direcaoOrdenacao: DirecaoOrdenacao.Asc, CancellationToken.None);

        usuarioAtualService.RestauranteId = restauranteB;
        var (itensDoB, _) = await repository.ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, categoriaId: null,
            ordenarPor: OrdenacaoProduto.Nome, direcaoOrdenacao: DirecaoOrdenacao.Asc, CancellationToken.None);

        Assert.Equal("Arroz", Assert.Single(itensDoA).Nome);
        Assert.Equal("Feijão", Assert.Single(itensDoB).Nome);
    }
}
