using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Abstractions.Repositories;

public interface IStockMovementRepository
{
    void Add(StockMovement movement);
}
