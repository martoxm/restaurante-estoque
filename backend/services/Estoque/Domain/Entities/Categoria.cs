using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Domain.Entities;

public class Categoria : Entity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public Guid RestauranteId { get; private set; }

    public DateTime CriadoEm { get; private set; }

    private Categoria()
    {
    }

    public static Categoria Criar(Guid restauranteId, string nome, DateTime criadoEm, string? descricao = null)
    {
        if (restauranteId == Guid.Empty)
            throw new DomainException("A categoria precisa pertencer a um restaurante.");

        ValidarNome(nome);

        return new Categoria
        {
            Nome = nome.Trim(),
            Descricao = descricao?.Trim(),
            RestauranteId = restauranteId,
            CriadoEm = criadoEm
        };
    }

    public void Atualizar(string nome, string? descricao = null)
    {
        ValidarNome(nome);

        Nome = nome.Trim();
        Descricao = descricao?.Trim();
    }

    public void GarantirQuePodeSerExcluida(bool possuiProdutos)
    {
        if (possuiProdutos)
            throw new DomainException("Não é possível excluir uma categoria que possui produtos vinculados.");
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da categoria é obrigatório.");

        if (nome.Trim().Length > 100)
            throw new DomainException("O nome da categoria deve ter no máximo 100 caracteres.");
    }
}
