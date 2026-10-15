using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Infrastructure.Identity;

namespace RestauranteEstoque.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    private Guid? CurrentRestaurantId => _currentUserService.RestaurantId;

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Product>().HasQueryFilter(product => product.RestaurantId == CurrentRestaurantId);
        modelBuilder.Entity<Category>().HasQueryFilter(category => category.RestaurantId == CurrentRestaurantId);
        modelBuilder.Entity<Supplier>().HasQueryFilter(supplier => supplier.RestaurantId == CurrentRestaurantId);
        modelBuilder.Entity<StockMovement>().HasQueryFilter(movement => movement.RestaurantId == CurrentRestaurantId);
    }
}
