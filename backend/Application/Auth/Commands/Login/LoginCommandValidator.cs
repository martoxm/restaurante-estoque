using FluentValidation;

namespace RestauranteEstoque.Application.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}
