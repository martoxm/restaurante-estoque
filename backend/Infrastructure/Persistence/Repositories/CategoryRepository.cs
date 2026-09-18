using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Include(category => category.Products)
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Category> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? name,
        SortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .Include(category => category.Products)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(category => category.Name.Contains(name));

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortDirection == SortDirection.Desc
            ? query.OrderByDescending(category => category.Name)
            : query.OrderBy(category => category.Name);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Category category)
    {
        _context.Categories.Add(category);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }
}
