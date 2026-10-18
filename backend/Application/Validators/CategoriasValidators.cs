using FluentValidation;
using RestauranteEstoque.Application.UseCases.Categorias;

namespace RestauranteEstoque.Application.Validators;

public class CriarCategoriaCommandValidator : AbstractValidator<CriarCategoriaCommand>
{
    public CriarCategoriaCommandValidator()
    {
        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");

        RuleFor(command => command.Descricao)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");
    }
}

public class ListarCategoriasQueryValidator : AbstractValidator<ListarCategoriasQuery>
{
    public ListarCategoriasQueryValidator()
    {
        RuleFor(query => query.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}

public class AtualizarCategoriaCommandValidator : AbstractValidator<AtualizarCategoriaCommand>
{
    public AtualizarCategoriaCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("O Id da categoria é obrigatório.");

        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");

        RuleFor(command => command.Descricao)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");
    }
}
