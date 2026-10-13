using FluentValidation;
using RestauranteEstoque.Application.UseCases.Suppliers;

namespace RestauranteEstoque.Application.Validators;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome do fornecedor é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do fornecedor deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Phone)
            .MaximumLength(20).WithMessage("O telefone deve ter no máximo 20 caracteres.");

        RuleFor(command => command.Email)
            .MaximumLength(200).WithMessage("O e-mail deve ter no máximo 200 caracteres.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .When(command => !string.IsNullOrWhiteSpace(command.Email));
    }
}

public class GetSuppliersQueryValidator : AbstractValidator<GetSuppliersQuery>
{
    public GetSuppliersQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("O Id do fornecedor é obrigatório.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome do fornecedor é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do fornecedor deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Phone)
            .MaximumLength(20).WithMessage("O telefone deve ter no máximo 20 caracteres.");

        RuleFor(command => command.Email)
            .MaximumLength(200).WithMessage("O e-mail deve ter no máximo 200 caracteres.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .When(command => !string.IsNullOrWhiteSpace(command.Email));
    }
}
