using MediatR;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Application.UseCases.StockMovements.Commands.CreateStockMovement;

public record CreateStockMovementCommand(
    Guid ProductId,
    StockMovementType Type,
    int Quantity,
    string? Notes = null) : IRequest<Guid>;
