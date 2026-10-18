using FluentValidation;
using RestauranteEstoque.Application.UseCases.Produtos;

namespace RestauranteEstoque.Application.Validators;

public class CriarProdutoCommandValidator : AbstractValidator<CriarProdutoCommand>
{
    public CriarProdutoCommandValidator()
    {
        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do produto deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Descricao)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(command => command.Preco)
            .GreaterThanOrEqualTo(0).WithMessage("O preço do produto não pode ser negativo.");

        RuleFor(command => command.CategoriaId)
            .NotEqual(Guid.Empty).WithMessage("O produto precisa estar vinculado a uma categoria.");

        RuleFor(command => command.QuantidadeInicial)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade em estoque não pode ser negativa.");
    }
}

public class ListarProdutosQueryValidator : AbstractValidator<ListarProdutosQuery>
{
    public ListarProdutosQueryValidator()
    {
        RuleFor(query => query.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}

public class AtualizarProdutoCommandValidator : AbstractValidator<AtualizarProdutoCommand>
{
    public AtualizarProdutoCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("O Id do produto é obrigatório.");

        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do produto deve ter no máximo 150 caracteres.");

        RuleFor(command => command.Descricao)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(command => command.Preco)
            .GreaterThanOrEqualTo(0).WithMessage("O preço do produto não pode ser negativo.");

        RuleFor(command => command.CategoriaId)
            .NotEqual(Guid.Empty).WithMessage("O produto precisa estar vinculado a uma categoria.");
    }
}
