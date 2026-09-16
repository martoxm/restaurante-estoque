using FluentValidation;

namespace RestauranteEstoque.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

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
        var user = await _userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant(), cancellationToken);
        return user is null;
    }
}
