using RestauranteEstoque.Application.UseCases.Produtos;
using RestauranteEstoque.Tests.Common;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Handlers;

public class CriarProdutoHandlerTests
{
    [Fact]
    public async Task Handle_ComDadosValidos_DeveCriarProdutoESalvarNoBanco()
    {
        using var context = TestDbContextFactory.Create();
        var usuarioAtualService = new FakeUsuarioAtualService { RestauranteId = DadosTeste.RestauranteId };
        var handler = new CriarProdutoHandler(new FakeProdutoRepository(context), context, usuarioAtualService, new FakeRelogio());
        var categoriaId = Guid.NewGuid();
        var command = new CriarProdutoCommand("Arroz", "Arroz branco tipo 1", 25.90m, categoriaId, QuantidadeInicial: 10);

        var produtoId = await handler.Handle(command, CancellationToken.None);

        var produtoSalvo = await context.Produtos.FindAsync(produtoId);
        Assert.NotNull(produtoSalvo);
        Assert.Equal("Arroz", produtoSalvo!.Nome);
        Assert.Equal(categoriaId, produtoSalvo.CategoriaId);
        Assert.Equal(10, produtoSalvo.QuantidadeEmEstoque.Valor);
    }
}
