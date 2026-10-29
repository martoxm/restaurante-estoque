using FluentValidation;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Identidade.Application.UseCases.Auth;

namespace RestauranteEstoque.Identidade.Application.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.");

        RuleFor(command => command.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}

public class RenovarTokenCommandValidator : AbstractValidator<RenovarTokenCommand>
{
    public RenovarTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty().WithMessage("O refresh token é obrigatório.");
    }
}

public class RegistrarCommandValidator: AbstractValidator<RegistrarCommand>
{
    private readonly IIdentityService _identityService;

    public RegistrarCommandValidator(IIdentityService identityService)
    {
        _identityService = identityService;

        RuleFor(command => command.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.");

        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .MustAsync(SerEmailUnico).WithMessage("Este e-mail já está cadastrado.");

        RuleFor(command => command.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.");

        RuleFor(command => command.NomeRestaurante)
            .NotEmpty().WithMessage("O nome do restaurante é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do restaurante deve ter no máximo 150 caracteres.");
    }

    private async Task<bool> SerEmailUnico(string email, CancellationToken cancellationToken)
    {
        var existe = await _identityService.EmailExisteAsync(email.Trim().ToLowerInvariant(), cancellationToken);
        return !existe;
    }
}
