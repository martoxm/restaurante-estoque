using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class TestDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public TestDbContext(DbContextOptions<TestDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    private Guid? CurrentRestaurantId => _currentUserService.RestaurantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasQueryFilter(product => product.RestaurantId == CurrentRestaurantId);
        modelBuilder.Entity<Category>().HasQueryFilter(category => category.RestaurantId == CurrentRestaurantId);
        modelBuilder.Entity<Supplier>().HasQueryFilter(supplier => supplier.RestaurantId == CurrentRestaurantId);
        modelBuilder.Entity<StockMovement>().HasQueryFilter(movement => movement.RestaurantId == CurrentRestaurantId);
    }
}

public static class TestDbContextFactory
{
    public static TestDbContext Create(ICurrentUserService? currentUserService = null)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options, currentUserService ?? new FakeCurrentUserService());
    }
}
