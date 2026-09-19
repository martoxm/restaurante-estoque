using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeStockMovementRepository : IStockMovementRepository
{
    private readonly TestDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public FakeStockMovementRepository(TestDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public void Add(StockMovement movement)
    {
        _context.StockMovements.Add(movement);
    }

    public async Task<bool> ExistsForProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.StockMovements.AnyAsync(
            movement => movement.ProductId == productId && movement.RestaurantId == _currentUserService.RestaurantId,
            cancellationToken);
    }
}
