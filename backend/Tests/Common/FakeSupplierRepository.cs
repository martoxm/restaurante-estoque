using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeSupplierRepository : ISupplierRepository
{
    private readonly TestDbContext _context;

    public FakeSupplierRepository(TestDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(supplier => supplier.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Supplier> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? name,
        SortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        IQueryable<Supplier> query = _context.Suppliers;

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(supplier => supplier.Name.Contains(name));

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortDirection == SortDirection.Desc
            ? query.OrderByDescending(supplier => supplier.Name)
            : query.OrderBy(supplier => supplier.Name);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
    }

    public void Update(Supplier supplier)
    {
        _context.Suppliers.Update(supplier);
    }

    public void Remove(Supplier supplier)
    {
        _context.Suppliers.Remove(supplier);
    }
}
