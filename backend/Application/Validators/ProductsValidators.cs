using FluentValidation;
using RestauranteEstoque.Application.UseCases.Products;

namespace RestauranteEstoque.Application.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do produto deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Description)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(command => command.Price)
            .GreaterThanOrEqualTo(0).WithMessage("O preço do produto não pode ser negativo.");

        RuleFor(command => command.CategoryId)
            .NotEqual(Guid.Empty).WithMessage("O produto precisa estar vinculado a uma categoria.");

        RuleFor(command => command.InitialQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade em estoque não pode ser negativa.");
    }
}

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}

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
