using FluentValidation;
using RestauranteEstoque.Application.UseCases.Restaurants.Commands.UpdateRestaurant;

namespace RestauranteEstoque.Application.Validators;

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
