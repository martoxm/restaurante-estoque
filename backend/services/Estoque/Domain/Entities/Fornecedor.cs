using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Domain.Entities;

public class Fornecedor : Entity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public Email? Email { get; private set; }

    public Guid RestauranteId { get; private set; }

    public DateTime CriadoEm { get; private set; }

    private Fornecedor()
    {
    }

    public static Fornecedor Criar(Guid restauranteId, string nome, DateTime criadoEm, string? telefone = null, Email? email = null)
    {
        if (restauranteId == Guid.Empty)
            throw new DomainException("O fornecedor precisa pertencer a um restaurante.");

        ValidarNome(nome);

        return new Fornecedor
        {
            Nome = nome.Trim(),
            Telefone = telefone?.Trim(),
            Email = email,
            RestauranteId = restauranteId,
            CriadoEm = criadoEm
        };
    }

    public void Atualizar(string nome, string? telefone = null, Email? email = null)
    {
        ValidarNome(nome);

        Nome = nome.Trim();
        Telefone = telefone?.Trim();
        Email = email;
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do fornecedor é obrigatório.");

        if (nome.Trim().Length > 150)
            throw new DomainException("O nome do fornecedor deve ter no máximo 150 caracteres.");
    }
}
