using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.StockMovements;

public interface IStockMovementRepository
{
    void Add(StockMovement movement);
}
