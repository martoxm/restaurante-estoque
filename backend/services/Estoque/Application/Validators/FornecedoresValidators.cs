using FluentValidation;
using RestauranteEstoque.Application.UseCases.Fornecedores;

namespace RestauranteEstoque.Application.Validators;

public class CriarFornecedorCommandValidator : AbstractValidator<CriarFornecedorCommand>
{
    public CriarFornecedorCommandValidator()
    {
        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome do fornecedor é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do fornecedor deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Telefone)
            .MaximumLength(20).WithMessage("O telefone deve ter no máximo 20 caracteres.");

        RuleFor(command => command.Email)
            .MaximumLength(200).WithMessage("O e-mail deve ter no máximo 200 caracteres.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .When(command => !string.IsNullOrWhiteSpace(command.Email));
    }
}

public class ListarFornecedoresQueryValidator : AbstractValidator<ListarFornecedoresQuery>
{
    public ListarFornecedoresQueryValidator()
    {
        RuleFor(query => query.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}

public class AtualizarFornecedorCommandValidator : AbstractValidator<AtualizarFornecedorCommand>
{
    public AtualizarFornecedorCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("O Id do fornecedor é obrigatório.");

        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome do fornecedor é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do fornecedor deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Telefone)
            .MaximumLength(20).WithMessage("O telefone deve ter no máximo 20 caracteres.");

        RuleFor(command => command.Email)
            .MaximumLength(200).WithMessage("O e-mail deve ter no máximo 200 caracteres.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .When(command => !string.IsNullOrWhiteSpace(command.Email));
    }
}
