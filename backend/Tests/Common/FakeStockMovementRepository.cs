using RestauranteEstoque.Application.StockMovements;
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
}
