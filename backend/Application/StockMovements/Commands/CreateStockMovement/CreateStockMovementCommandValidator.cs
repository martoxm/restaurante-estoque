using FluentValidation;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Application.StockMovements.Commands.CreateStockMovement;

public class CreateStockMovementCommandValidator : AbstractValidator<CreateStockMovementCommand>
{
    public CreateStockMovementCommandValidator()
    {
        RuleFor(command => command.ProductId)
            .NotEqual(Guid.Empty).WithMessage("A movimentação precisa estar vinculada a um produto.");

        RuleFor(command => command.Type)
            .IsInEnum().WithMessage("O tipo de movimentação deve ser Entrada ou Saida.");

        RuleFor(command => command.Quantity)
            .GreaterThan(0).WithMessage("A quantidade movimentada deve ser maior que zero.");

        RuleFor(command => command.Notes)
            .MaximumLength(500).WithMessage("As observações devem ter no máximo 500 caracteres.");
    }
}
