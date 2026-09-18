using FluentValidation;
using RestauranteEstoque.Application.UseCases.Suppliers.Commands.CreateSupplier;

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
