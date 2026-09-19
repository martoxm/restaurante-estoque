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
        using var context = TestDbContextFactory.Create();
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoria = Category.Create("Grãos", CreatedAt);
        var produto = Product.Create("Arroz", null, 10m, categoria.Id, CreatedAt, restaurantId: restauranteB);
        var movimentacao = StockMovement.Create(produto.Id, StockMovementType.Entrada, 5, CreatedAt, restaurantId: restauranteB);
        context.Categories.Add(categoria);
        context.Products.Add(produto);
        context.StockMovements.Add(movimentacao);
        await context.SaveChangesAsync();

        var currentUserService = new FakeCurrentUserService { RestaurantId = restauranteA };
        var repository = new FakeStockMovementRepository(context, currentUserService);

        var existe = await repository.ExistsForProductAsync(produto.Id, CancellationToken.None);

        Assert.False(existe);
    }

    [Fact]
    public async Task ExistsForProductAsync_ComMovimentacaoDoProprioRestaurante_DeveRetornarTrue()
    {
        using var context = TestDbContextFactory.Create();
        var restauranteA = Guid.NewGuid();
        var categoria = Category.Create("Grãos", CreatedAt);
        var produto = Product.Create("Arroz", null, 10m, categoria.Id, CreatedAt, restaurantId: restauranteA);
        var movimentacao = StockMovement.Create(produto.Id, StockMovementType.Entrada, 5, CreatedAt, restaurantId: restauranteA);
        context.Categories.Add(categoria);
        context.Products.Add(produto);
        context.StockMovements.Add(movimentacao);
        await context.SaveChangesAsync();

        var currentUserService = new FakeCurrentUserService { RestaurantId = restauranteA };
        var repository = new FakeStockMovementRepository(context, currentUserService);

        var existe = await repository.ExistsForProductAsync(produto.Id, CancellationToken.None);

        Assert.True(existe);
    }
}
