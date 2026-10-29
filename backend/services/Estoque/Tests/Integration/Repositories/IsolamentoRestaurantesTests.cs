using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Produtos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Domain.Services;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Infrastructure.Persistence;
using RestauranteEstoque.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Tests.Common;
using RestauranteEstoque.Tests.Integration.Fixtures;

namespace RestauranteEstoque.Tests.Integration.Repositories;

[Collection(nameof(SqlServerCollection))]
public sealed class IsolamentoRestaurantesTests(SqlServerFixture fixture)
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [DockerFact]
    public async Task Produto_ListarPaginado_DeveTrazerSoOsDoRestauranteAtual()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteA, restauranteB) = await CriarDoisRestaurantesAsync();
        await CriarProdutoAsync(restauranteA, "Arroz");
        await CriarProdutoAsync(restauranteB, "Feijão");

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteA));
        var (itens, totalItens) = await new ProdutoRepository(contexto).ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, categoriaId: null,
            ordenarPor: OrdenacaoProduto.Nome, direcaoOrdenacao: DirecaoOrdenacao.Asc, CancellationToken.None);

        Assert.Equal(1, totalItens);
        Assert.Equal("Arroz", Assert.Single(itens).Nome);
    }

    [DockerFact]
    public async Task Produto_ObterPorId_DeOutroRestaurante_DeveDevolverNull()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteA, restauranteB) = await CriarDoisRestaurantesAsync();
        var produtoDoB = await CriarProdutoAsync(restauranteB, "Feijão");

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteA));
        var produto = await new ProdutoRepository(contexto).ObterPorIdAsync(produtoDoB.Id, CancellationToken.None);

        Assert.Null(produto);
    }

    [DockerFact]
    public async Task Categoria_ListarPaginado_DeveTrazerSoAsDoRestauranteAtual_ComAContagemCerta()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteA, restauranteB) = await CriarDoisRestaurantesAsync();
        var categoriaDoA = await CriarCategoriaAsync(restauranteA);
        await CriarProdutoAsync(restauranteA, "Arroz", categoriaDoA);
        await CriarProdutoAsync(restauranteA, "Feijão", categoriaDoA);
        await CriarProdutoAsync(restauranteB, "Milho");

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteA));
        var (itens, totalItens) = await new CategoriaRepository(contexto).ListarPaginadoAsync(
            pagina: 1, tamanhoPagina: 10, nome: null, DirecaoOrdenacao.Asc, CancellationToken.None);

        Assert.Equal(1, totalItens);
        var item = Assert.Single(itens);
        Assert.Equal(categoriaDoA.Id, item.Categoria.Id);
        Assert.Equal(2, item.QuantidadeProdutos);
    }

    [DockerFact]
    public async Task Fornecedor_ObterPorId_DeOutroRestaurante_DeveDevolverNull()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteA, restauranteB) = await CriarDoisRestaurantesAsync();
        var fornecedorDoB = Fornecedor.Criar(restauranteB, "Distribuidora", CriadoEm);
        await using (var contextoEscrita = fixture.CriarContexto(UsuarioDo(restauranteB)))
        {
            contextoEscrita.Fornecedores.Add(fornecedorDoB);
            await contextoEscrita.SaveChangesAsync();
        }

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteA));
        var fornecedor = await new FornecedorRepository(contexto).ObterPorIdAsync(fornecedorDoB.Id, CancellationToken.None);

        Assert.Null(fornecedor);
    }

    [DockerFact]
    public async Task MovimentacaoEstoque_ExisteParaProduto_DeOutroRestaurante_DeveDevolverFalse()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteA, restauranteB) = await CriarDoisRestaurantesAsync();
        var categoriaDoB = DadosTeste.CriarCategoria(restauranteB);
        var produtoDoB = Produto.Criar(restauranteB, "Feijão", null, Dinheiro.Criar(8m), categoriaDoB, CriadoEm);
        var movimentacao = MovimentadorEstoque.Movimentar(produtoDoB, TipoMovimentacaoEstoque.Entrada, Quantidade.Criar(5), CriadoEm);

        await using (var contextoEscrita = fixture.CriarContexto(UsuarioDo(restauranteB)))
        {
            contextoEscrita.Categorias.Add(categoriaDoB);
            contextoEscrita.Produtos.Add(produtoDoB);
            contextoEscrita.MovimentacoesEstoque.Add(movimentacao);
            await contextoEscrita.SaveChangesAsync();
        }

        await using var contextoDoA = fixture.CriarContexto(UsuarioDo(restauranteA));
        await using var contextoDoB = fixture.CriarContexto(UsuarioDo(restauranteB));

        Assert.True(await new MovimentacaoEstoqueRepository(contextoDoB).ExisteParaProdutoAsync(produtoDoB.Id, CancellationToken.None));
        Assert.False(await new MovimentacaoEstoqueRepository(contextoDoA).ExisteParaProdutoAsync(produtoDoB.Id, CancellationToken.None));
    }

    [DockerFact]
    public async Task TrocarORestauranteNoMesmoContexto_DeveMudarOResultado()
    {
        if (!fixture.EstaDisponivel)
            return;

        var (restauranteA, restauranteB) = await CriarDoisRestaurantesAsync();
        var produtoDoA = await CriarProdutoAsync(restauranteA, "Arroz");

        var usuario = UsuarioDo(restauranteA);
        await using var contexto = fixture.CriarContexto(usuario);
        var repository = new ProdutoRepository(contexto);

        var comoA = await repository.ObterPorIdAsync(produtoDoA.Id, CancellationToken.None);
        usuario.RestauranteId = restauranteB;
        var comoB = await repository.ObterPorIdAsync(produtoDoA.Id, CancellationToken.None);

        Assert.NotNull(comoA);
        Assert.Null(comoB);
    }

    private static FakeUsuarioAtualService UsuarioDo(Guid restauranteId) =>
        new() { RestauranteId = restauranteId };

    private static Task<(Guid A, Guid B)> CriarDoisRestaurantesAsync() =>
        Task.FromResult((Guid.NewGuid(), Guid.NewGuid()));

    private async Task<Categoria> CriarCategoriaAsync(Guid restauranteId)
    {
        var categoria = DadosTeste.CriarCategoria(restauranteId);

        await using var contexto = fixture.CriarContexto(UsuarioDo(restauranteId));
        contexto.Categorias.Add(categoria);
        await contexto.SaveChangesAsync();

        return categoria;
    }

    private async Task<Produto> CriarProdutoAsync(Guid restauranteId, string nome, Categoria? categoria = null)
    {
        categoria ??= await CriarCategoriaAsync(restauranteId);
        var produto = Produto.Criar(restauranteId, nome, null, Dinheiro.Criar(10m), categoria, CriadoEm);

        await using AppDbContext contexto = fixture.CriarContexto(UsuarioDo(restauranteId));
        contexto.Produtos.Add(produto);
        await contexto.SaveChangesAsync();

        return produto;
    }
}
