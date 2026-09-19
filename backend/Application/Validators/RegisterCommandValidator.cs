using FluentValidation;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.UseCases.Auth.Commands.Register;

namespace RestauranteEstoque.Application.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandValidator(IIdentityService identityService)
    {
        _identityService = identityService;

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.");

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .MustAsync(BeUniqueEmail).WithMessage("Este e-mail já está cadastrado.");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var exists = await _identityService.EmailExistsAsync(email.Trim().ToLowerInvariant(), cancellationToken);
        return !exists;
    }
}
