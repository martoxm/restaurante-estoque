using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeRestaurantRepository : IRestaurantRepository
{
    private readonly TestDbContext _context;

    public FakeRestaurantRepository(TestDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Restaurants
            .FirstOrDefaultAsync(restaurant => restaurant.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Restaurant> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? name,
        SortDirection sortDirection,
        CancellationToken cancellationToken)
    {
        var query = _context.Restaurants.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(restaurant => restaurant.Name.Contains(name));

        var totalCount = await query.CountAsync(cancellationToken);

        query = sortDirection == SortDirection.Desc
            ? query.OrderByDescending(restaurant => restaurant.Name)
            : query.OrderBy(restaurant => restaurant.Name);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Restaurant restaurant)
    {
        _context.Restaurants.Add(restaurant);
    }

    public void Update(Restaurant restaurant)
    {
        _context.Restaurants.Update(restaurant);
    }
}
