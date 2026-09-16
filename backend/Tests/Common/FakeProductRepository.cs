using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Products;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Products;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeProductRepository : IProductRepository
{
    private readonly TestDbContext _context;

    public FakeProductRepository(TestDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? name,
        Guid? categoryId,
        ProductSortBy sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(product => product.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(product => product.Name.Contains(name));

        if (categoryId.HasValue)
            query = query.Where(product => product.CategoryId == categoryId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await ApplySort(query, sortBy, sortDirection)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    private static IQueryable<Product> ApplySort(IQueryable<Product> query, ProductSortBy sortBy, SortDirection sortDirection)
    {
        return (sortBy, sortDirection) switch
        {
            (ProductSortBy.Name, SortDirection.Desc) => query.OrderByDescending(product => product.Name),
            (ProductSortBy.Price, SortDirection.Asc) => query.OrderBy(product => product.Price),
            (ProductSortBy.Price, SortDirection.Desc) => query.OrderByDescending(product => product.Price),
            (ProductSortBy.QuantityInStock, SortDirection.Asc) => query.OrderBy(product => product.QuantityInStock),
            (ProductSortBy.QuantityInStock, SortDirection.Desc) => query.OrderByDescending(product => product.QuantityInStock),
            _ => query.OrderBy(product => product.Name)
        };
    }
}
