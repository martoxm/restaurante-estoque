using RestauranteEstoque.Application.StockMovements;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly AppDbContext _context;

    public StockMovementRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(StockMovement movement)
    {
        _context.StockMovements.Add(movement);
    }
}
