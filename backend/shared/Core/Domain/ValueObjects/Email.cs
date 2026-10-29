using System.Text.RegularExpressions;
using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Core.Domain.ValueObjects;

public sealed partial record Email
{
    public const int TamanhoMaximo = 200;

    public string Valor { get; }

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Email Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("O e-mail é obrigatório.");

        var normalizado = valor.Trim().ToLowerInvariant();

        if (normalizado.Length > TamanhoMaximo)
            throw new DomainException($"O e-mail deve ter no máximo {TamanhoMaximo} caracteres.");

        if (!FormatoValido().IsMatch(normalizado))
            throw new DomainException("O e-mail informado não é válido.");

        return new Email(normalizado);
    }

    public static Email? CriarOuNulo(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : Criar(valor);

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex FormatoValido();

    public override string ToString() => Valor;
}
