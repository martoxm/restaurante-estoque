using FluentValidation;
using RestauranteEstoque.Application.UseCases.Restaurants;

namespace RestauranteEstoque.Application.Validators;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome do restaurante é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do restaurante deve ter no máximo 150 caracteres.");
    }
}

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

public class UpdateRestaurantCommandValidator : AbstractValidator<UpdateRestaurantCommand>
{
    public UpdateRestaurantCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty).WithMessage("O Id do restaurante é obrigatório.");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome do restaurante é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do restaurante deve ter no máximo 150 caracteres.");
    }
}
