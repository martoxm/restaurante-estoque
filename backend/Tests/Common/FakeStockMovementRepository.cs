using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeStockMovementRepository : IStockMovementRepository
{
    private readonly TestDbContext _context;

    public FakeStockMovementRepository(TestDbContext context)
    {
        _context = context;
    }

    public void Add(StockMovement movement)
    {
        _context.StockMovements.Add(movement);
    }

    public async Task<bool> ExistsForProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.StockMovements.AnyAsync(movement => movement.ProductId == productId, cancellationToken);
    }
}
