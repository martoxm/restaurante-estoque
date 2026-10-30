using FluentValidation;
using RestauranteEstoque.Vendas.Application.UseCases.Vendas;

namespace RestauranteEstoque.Vendas.Application.Validators;

public class CriarVendaCommandValidator : AbstractValidator<CriarVendaCommand>
{
    public CriarVendaCommandValidator()
    {
        RuleFor(command => command.Itens)
            .NotEmpty().WithMessage("A venda precisa ter pelo menos um item.");

        RuleFor(command => command.Itens)
            .Must(itens => itens.Select(item => item.ProdutoId).Distinct().Count() == itens.Count)
            .WithMessage("Um produto não pode aparecer mais de uma vez na mesma venda.")
            .When(command => command.Itens is not null);

        RuleForEach(command => command.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.ProdutoId)
                .NotEqual(Guid.Empty).WithMessage("Escolha o produto.");

            item.RuleFor(i => i.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade precisa ser maior que zero.");
        });
    }
}

public class ListarVendasQueryValidator : AbstractValidator<ListarVendasQuery>
{
    public ListarVendasQueryValidator()
    {
        RuleFor(query => query.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}
