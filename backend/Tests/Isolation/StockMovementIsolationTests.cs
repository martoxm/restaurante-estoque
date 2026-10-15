using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Isolation;

public class StockMovementIsolationTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ExistsForProductAsync_ComMovimentacaoDeOutroRestaurante_DeveRetornarFalse()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoria = Category.Create("Grãos", CreatedAt, restaurantId: restauranteB);
        var produto = Product.Create("Arroz", null, 10m, categoria.Id, CreatedAt, restaurantId: restauranteB);
        var movimentacao = StockMovement.Create(produto.Id, StockMovementType.Entrada, 5, CreatedAt, restaurantId: restauranteB);
        context.Categories.Add(categoria);
        context.Products.Add(produto);
        context.StockMovements.Add(movimentacao);
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA;
        var repository = new FakeStockMovementRepository(context);

        var existe = await repository.ExistsForProductAsync(produto.Id, CancellationToken.None);

        Assert.False(existe);
    }

    [Fact]
    public async Task ExistsForProductAsync_ComMovimentacaoDoProprioRestaurante_DeveRetornarTrue()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Guid.NewGuid();
        var categoria = Category.Create("Grãos", CreatedAt, restaurantId: restauranteA);
        var produto = Product.Create("Arroz", null, 10m, categoria.Id, CreatedAt, restaurantId: restauranteA);
        var movimentacao = StockMovement.Create(produto.Id, StockMovementType.Entrada, 5, CreatedAt, restaurantId: restauranteA);
        context.Categories.Add(categoria);
        context.Products.Add(produto);
        context.StockMovements.Add(movimentacao);
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA;
        var repository = new FakeStockMovementRepository(context);

        var existe = await repository.ExistsForProductAsync(produto.Id, CancellationToken.None);

        Assert.True(existe);
    }
}
