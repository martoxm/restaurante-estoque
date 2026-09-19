using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Products;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Isolation;

public class ProductIsolationTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task GetPagedAsync_DeveRetornarSoProdutosDoRestauranteAtual()
    {
        using var context = TestDbContextFactory.Create();
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoria = Category.Create("Grãos", CreatedAt);
        context.Categories.Add(categoria);
        context.Products.Add(Product.Create("Arroz", null, 10m, categoria.Id, CreatedAt, restaurantId: restauranteA));
        context.Products.Add(Product.Create("Feijão", null, 8m, categoria.Id, CreatedAt, restaurantId: restauranteB));
        await context.SaveChangesAsync();

        var currentUserService = new FakeCurrentUserService { RestaurantId = restauranteA };
        var repository = new FakeProductRepository(context, currentUserService);

        var (items, totalCount) = await repository.GetPagedAsync(
            page: 1, pageSize: 10, name: null, categoryId: null,
            sortBy: ProductSortBy.Name, sortDirection: SortDirection.Asc, CancellationToken.None);

        Assert.Equal(1, totalCount);
        Assert.Equal("Arroz", Assert.Single(items).Name);
    }

    [Fact]
    public async Task GetByIdAsync_ComProdutoDeOutroRestaurante_DeveRetornarNull()
    {
        using var context = TestDbContextFactory.Create();
        var restauranteA = Guid.NewGuid();
        var restauranteB = Guid.NewGuid();
        var categoria = Category.Create("Grãos", CreatedAt);
        var produtoDoRestauranteB = Product.Create("Arroz", null, 10m, categoria.Id, CreatedAt, restaurantId: restauranteB);
        context.Categories.Add(categoria);
        context.Products.Add(produtoDoRestauranteB);
        await context.SaveChangesAsync();

        var currentUserService = new FakeCurrentUserService { RestaurantId = restauranteA };
        var repository = new FakeProductRepository(context, currentUserService);

        var produto = await repository.GetByIdAsync(produtoDoRestauranteB.Id, CancellationToken.None);

        Assert.Null(produto);
    }

    [Fact]
    public async Task GetByIdAsync_ComProdutoDoProprioRestaurante_DeveRetornarOProduto()
    {
        using var context = TestDbContextFactory.Create();
        var restauranteA = Guid.NewGuid();
        var categoria = Category.Create("Grãos", CreatedAt);
        var produto = Product.Create("Arroz", null, 10m, categoria.Id, CreatedAt, restaurantId: restauranteA);
        context.Categories.Add(categoria);
        context.Products.Add(produto);
        await context.SaveChangesAsync();

        var currentUserService = new FakeCurrentUserService { RestaurantId = restauranteA };
        var repository = new FakeProductRepository(context, currentUserService);

        var produtoEncontrado = await repository.GetByIdAsync(produto.Id, CancellationToken.None);

        Assert.NotNull(produtoEncontrado);
    }
}
