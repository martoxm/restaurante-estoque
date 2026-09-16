using RestauranteEstoque.Application.Products.Commands.CreateProduct;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class CreateProductHandlerTests
{
    [Fact]
    public async Task Handle_ComDadosValidos_DeveCriarProdutoESalvarNoBanco()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new CreateProductHandler(new FakeProductRepository(context), context);
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand("Arroz", "Arroz branco tipo 1", 25.90m, categoryId, InitialQuantity: 10);

        var productId = await handler.Handle(command, CancellationToken.None);

        var produtoSalvo = await context.Products.FindAsync(productId);
        Assert.NotNull(produtoSalvo);
        Assert.Equal("Arroz", produtoSalvo!.Name);
        Assert.Equal(categoryId, produtoSalvo.CategoryId);
        Assert.Equal(10, produtoSalvo.QuantityInStock);
    }
}
