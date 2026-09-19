using FluentValidation;
using RestauranteEstoque.Application.UseCases.Restaurants.Commands.CreateRestaurant;

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
