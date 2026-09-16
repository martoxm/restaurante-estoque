using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Domain.Entities;

public class StockMovement : Entity
{
    public Guid ProductId { get; private set; }
    public Product? Product { get; private set; }
    public StockMovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public DateTime MovementDate { get; private set; }
    public string? Notes { get; private set; }

    private StockMovement()
    {
    }

    public static StockMovement Create(Guid productId, StockMovementType type, int quantity, DateTime movementDate, string? notes = null)
    {
        ValidateProductId(productId);
        ValidateQuantity(quantity);

        return new StockMovement
        {
            ProductId = productId,
            Type = type,
            Quantity = quantity,
            Notes = notes?.Trim(),
            MovementDate = movementDate
        };
    }

    private static void ValidateProductId(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new DomainException("A movimentação precisa estar vinculada a um produto.");
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("A quantidade movimentada deve ser maior que zero.");
    }
}
