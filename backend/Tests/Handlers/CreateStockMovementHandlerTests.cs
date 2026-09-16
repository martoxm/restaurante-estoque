using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.StockMovements.Commands.CreateStockMovement;
using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class CreateStockMovementHandlerTests
{
    [Fact]
    public async Task Handle_Entrada_DeveAumentarEstoqueERegistrarMovimentacao()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Grãos");
        var product = Product.Create("Arroz", null, 10m, category.Id, initialQuantity: 5);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new CreateStockMovementHandler(
            new FakeStockMovementRepository(context),
            new FakeProductRepository(context),
            context,
            new FakeClock());
        var command = new CreateStockMovementCommand(product.Id, StockMovementType.Entrada, 3, "Compra do mês");

        var movementId = await handler.Handle(command, CancellationToken.None);

        var produtoAtualizado = await context.Products.FindAsync(product.Id);
        var movimentacaoSalva = await context.StockMovements.FindAsync(movementId);
        Assert.Equal(8, produtoAtualizado!.QuantityInStock);
        Assert.NotNull(movimentacaoSalva);
        Assert.Equal(StockMovementType.Entrada, movimentacaoSalva!.Type);
    }

    [Fact]
    public async Task Handle_Saida_ComEstoqueSuficiente_DeveDiminuirEstoque()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Grãos");
        var product = Product.Create("Arroz", null, 10m, category.Id, initialQuantity: 5);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new CreateStockMovementHandler(
            new FakeStockMovementRepository(context),
            new FakeProductRepository(context),
            context,
            new FakeClock());
        var command = new CreateStockMovementCommand(product.Id, StockMovementType.Saida, 3);

        await handler.Handle(command, CancellationToken.None);

        var produtoAtualizado = await context.Products.FindAsync(product.Id);
        Assert.Equal(2, produtoAtualizado!.QuantityInStock);
    }

    [Fact]
    public async Task Handle_Saida_ComEstoqueInsuficiente_DeveLancarDomainException()
    {
        using var context = TestDbContextFactory.Create();
        var category = Category.Create("Grãos");
        var product = Product.Create("Arroz", null, 10m, category.Id, initialQuantity: 5);
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new CreateStockMovementHandler(
            new FakeStockMovementRepository(context),
            new FakeProductRepository(context),
            context,
            new FakeClock());
        var command = new CreateStockMovementCommand(product.Id, StockMovementType.Saida, 100);

        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ComProdutoInexistente_DeveLancarNotFoundException()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new CreateStockMovementHandler(
            new FakeStockMovementRepository(context),
            new FakeProductRepository(context),
            context,
            new FakeClock());
        var command = new CreateStockMovementCommand(Guid.NewGuid(), StockMovementType.Entrada, 10);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
