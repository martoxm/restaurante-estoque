using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Isolation;

public class RestaurantIsolationTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task GetPagedAsync_DeveRetornarSoORestauranteAtual()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Restaurant.Create("Restaurante A", CreatedAt);
        var restauranteB = Restaurant.Create("Restaurante B", CreatedAt);
        context.Restaurants.AddRange(restauranteA, restauranteB);
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA.Id;
        var repository = new FakeRestaurantRepository(context);

        var (items, totalCount) = await repository.GetPagedAsync(
            page: 1, pageSize: 10, name: null, sortDirection: SortDirection.Asc, CancellationToken.None);

        Assert.Equal(1, totalCount);
        Assert.Equal("Restaurante A", Assert.Single(items).Name);
    }

    [Fact]
    public async Task GetByIdAsync_ComIdDeOutroRestaurante_DeveRetornarNull()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Restaurant.Create("Restaurante A", CreatedAt);
        var restauranteB = Restaurant.Create("Restaurante B", CreatedAt);
        context.Restaurants.AddRange(restauranteA, restauranteB);
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA.Id;
        var repository = new FakeRestaurantRepository(context);

        var restaurante = await repository.GetByIdAsync(restauranteB.Id, CancellationToken.None);

        Assert.Null(restaurante);
    }
}
