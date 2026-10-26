using Microsoft.AspNetCore.Identity;

namespace RestauranteEstoque.Infrastructure.Identity;

public class IdentityErrorDescriberPtBr : IdentityErrorDescriber
{
    public override IdentityError DefaultError() =>
        Erro(nameof(DefaultError), "Não foi possível concluir a operação.");

    public override IdentityError PasswordTooShort(int length) =>
        Erro(nameof(PasswordTooShort), $"A senha deve ter pelo menos {length} caracteres.");

    public override IdentityError PasswordRequiresDigit() =>
        Erro(nameof(PasswordRequiresDigit), "A senha deve ter pelo menos um número.");

    public override IdentityError PasswordRequiresLower() =>
        Erro(nameof(PasswordRequiresLower), "A senha deve ter pelo menos uma letra minúscula.");

    public override IdentityError PasswordRequiresUpper() =>
        Erro(nameof(PasswordRequiresUpper), "A senha deve ter pelo menos uma letra maiúscula.");

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        Erro(nameof(PasswordRequiresNonAlphanumeric), "A senha deve ter pelo menos um caractere especial.");

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        Erro(nameof(PasswordRequiresUniqueChars), $"A senha deve ter pelo menos {uniqueChars} caracteres diferentes.");

    public override IdentityError InvalidEmail(string? email) =>
        Erro(nameof(InvalidEmail), "O e-mail informado não é válido.");

    public override IdentityError DuplicateEmail(string email) =>
        Erro(nameof(DuplicateEmail), "Este e-mail já está cadastrado.");

    public override IdentityError InvalidUserName(string? userName) =>
        Erro(nameof(InvalidUserName), "O e-mail informado não é válido.");

    public override IdentityError DuplicateUserName(string userName) =>
        Erro(nameof(DuplicateUserName), "Este e-mail já está cadastrado.");

    private static IdentityError Erro(string codigo, string descricao) =>
        new() { Code = codigo, Description = descricao };
}
