using FluentValidation;
using RestauranteEstoque.Identidade.Application.UseCases.Restaurantes;

namespace RestauranteEstoque.Identidade.Application.Validators;

public class ListarRestaurantesQueryValidator : AbstractValidator<ListarRestaurantesQuery>
{
    public ListarRestaurantesQueryValidator()
    {
        RuleFor(query => query.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}

public class AtualizarRestauranteCommandValidator : AbstractValidator<AtualizarRestauranteCommand>
{
    public AtualizarRestauranteCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("O Id do restaurante é obrigatório.");

        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome do restaurante é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do restaurante deve ter no máximo 150 caracteres.");
    }
}
