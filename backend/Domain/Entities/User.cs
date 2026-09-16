using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    private User()
    {
    }

    public static User Create(string name, string email, string passwordHash)
    {
        ValidateName(name);
        ValidateEmail(email);
        ValidatePasswordHash(passwordHash);

        return new User
        {
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash
        };
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do usuário é obrigatório.");
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("O e-mail é obrigatório.");

        if (!email.Contains('@') || !email.Contains('.'))
            throw new DomainException("O e-mail informado não é válido.");
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("A senha (hash) é obrigatória.");
    }
}
