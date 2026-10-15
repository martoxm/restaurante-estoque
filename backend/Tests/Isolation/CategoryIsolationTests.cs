using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Isolation;

public class CategoryIsolationTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task GetPagedAsync_DeveRetornarSoCategoriasDoRestauranteAtual()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        context.Categories.Add(Category.Create("Grãos", CreatedAt, restaurantId: restauranteA));
        context.Categories.Add(Category.Create("Bebidas", CreatedAt, restaurantId: restauranteB));
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA;
        var repository = new FakeCategoryRepository(context);

        var (items, totalCount) = await repository.GetPagedAsync(
            page: 1, pageSize: 10, name: null, sortDirection: SortDirection.Asc, CancellationToken.None);

        Assert.Equal(1, totalCount);
        Assert.Equal("Grãos", Assert.Single(items).Name);
    }

    [Fact]
    public async Task GetByIdAsync_ComCategoriaDeOutroRestaurante_DeveRetornarNull()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoriaDoRestauranteB = Category.Create("Bebidas", CreatedAt, restaurantId: restauranteB);
        context.Categories.Add(categoriaDoRestauranteB);
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA;
        var repository = new FakeCategoryRepository(context);

        var categoria = await repository.GetByIdAsync(categoriaDoRestauranteB.Id, CancellationToken.None);

        Assert.Null(categoria);
    }
}
