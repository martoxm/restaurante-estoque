using FluentValidation;
using RestauranteEstoque.Application.UseCases.Restaurants.Queries.GetRestaurants;

namespace RestauranteEstoque.Application.Validators;

public class GetRestaurantsQueryValidator : AbstractValidator<GetRestaurantsQuery>
{
    public GetRestaurantsQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100).WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}
