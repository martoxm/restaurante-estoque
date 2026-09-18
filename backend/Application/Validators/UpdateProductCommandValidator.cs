using FluentValidation;
using RestauranteEstoque.Application.UseCases.Products.Commands.UpdateProduct;

namespace RestauranteEstoque.Application.Validators;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("O Id do produto é obrigatório.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do produto deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Description)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0).WithMessage("O preço do produto não pode ser negativo.");

        RuleFor(command => command.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("O produto precisa estar vinculado a uma categoria.");
    }
}
