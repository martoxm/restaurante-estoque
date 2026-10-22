using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.UseCases.Produtos;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Handlers;

public class CriarProdutoHandlerTests
{
    private static readonly DateTime CriadoEm = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_ComDadosValidos_DeveCriarProdutoESalvarNoBanco()
    {
        using var context = TestDbContextFactory.Create();
        var categoria = DadosTeste.CriarCategoria();
        context.Categorias.Add(categoria);
        await context.SaveChangesAsync();
        var handler = CriarHandler(context);
        var command = new CriarProdutoCommand("Arroz", "Arroz branco tipo 1", 25.90m, categoria.Id, QuantidadeInicial: 10);

        var produtoId = await handler.Handle(command, CancellationToken.None);

        var produtoSalvo = await context.Produtos.FindAsync(produtoId);
        Assert.NotNull(produtoSalvo);
        Assert.Equal("Arroz", produtoSalvo!.Nome);
        Assert.Equal(categoria.Id, produtoSalvo.CategoriaId);
        Assert.Equal(10, produtoSalvo.QuantidadeEmEstoque.Valor);
    }

    [Fact]
    public async Task Handle_ComCategoriaInexistente_DeveLancarNaoEncontradoException()
    {
        using var context = TestDbContextFactory.Create();
        var handler = CriarHandler(context);
        var command = new CriarProdutoCommand("Arroz", null, 10m, Guid.NewGuid(), QuantidadeInicial: 0);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Empty(context.Produtos);
    }

    [Fact]
    public async Task Handle_ComCategoriaDeOutroRestaurante_DeveLancarNaoEncontradoException()
    {
        using var context = TestDbContextFactory.Create();
        var categoriaDeOutroRestaurante = Categoria.Criar(Guid.NewGuid(), "Grãos", CriadoEm);
        context.Categorias.Add(categoriaDeOutroRestaurante);
        await context.SaveChangesAsync();
        var handler = CriarHandler(context);
        var command = new CriarProdutoCommand("Arroz", null, 10m, categoriaDeOutroRestaurante.Id, QuantidadeInicial: 0);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Empty(context.Produtos);
    }

    private static CriarProdutoHandler CriarHandler(TestDbContext context) =>
        new(
            new FakeProdutoRepository(context),
            new FakeCategoriaRepository(context),
            context,
            new FakeUsuarioAtualService { RestauranteId = DadosTeste.RestauranteId },
            new FakeRelogio());
}
