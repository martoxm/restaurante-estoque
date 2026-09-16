using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Suppliers;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
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
        var query = _context.Suppliers.AsQueryable();

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
