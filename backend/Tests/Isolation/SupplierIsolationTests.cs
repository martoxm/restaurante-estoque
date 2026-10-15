using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Isolation;

public class SupplierIsolationTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task GetPagedAsync_DeveRetornarSoFornecedoresDoRestauranteAtual()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        context.Suppliers.Add(Supplier.Create("Distribuidora A", CreatedAt, restaurantId: restauranteA));
        context.Suppliers.Add(Supplier.Create("Distribuidora B", CreatedAt, restaurantId: restauranteB));
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA;
        var repository = new FakeSupplierRepository(context);

        var (items, totalCount) = await repository.GetPagedAsync(
            page: 1, pageSize: 10, name: null, sortDirection: SortDirection.Asc, CancellationToken.None);

        Assert.Equal(1, totalCount);
        Assert.Equal("Distribuidora A", Assert.Single(items).Name);
    }

    [Fact]
    public async Task GetByIdAsync_ComFornecedorDeOutroRestaurante_DeveRetornarNull()
    {
        var currentUserService = new FakeCurrentUserService();
        using var context = TestDbContextFactory.Create(currentUserService);
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var fornecedorDoRestauranteB = Supplier.Create("Distribuidora B", CreatedAt, restaurantId: restauranteB);
        context.Suppliers.Add(fornecedorDoRestauranteB);
        await context.SaveChangesAsync();

        currentUserService.RestaurantId = restauranteA;
        var repository = new FakeSupplierRepository(context);

        var fornecedor = await repository.GetByIdAsync(fornecedorDoRestauranteB.Id, CancellationToken.None);

        Assert.Null(fornecedor);
    }
}
