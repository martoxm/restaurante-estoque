using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Identidade.Domain.Entities;

public class Restaurante : Entity
{
    public string Nome { get; private set; } = string.Empty;

    public DateTime CriadoEm { get; private set; }

    private Restaurante()
    {
    }

    public static Restaurante Criar(string nome, DateTime criadoEm)
    {
        ValidarNome(nome);

        return new Restaurante
        {
            Nome = nome.Trim(),
            CriadoEm = criadoEm
        };
    }

    public void Atualizar(string nome)
    {
        ValidarNome(nome);

        Nome = nome.Trim();
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do restaurante é obrigatório.");

        if (nome.Trim().Length > 150)
            throw new DomainException("O nome do restaurante deve ter no máximo 150 caracteres.");
    }
}
