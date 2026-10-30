using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Vendas.Domain.Entities;

public class ProdutoCatalogo : Entity
{
    public const int NomeMaxLength = 150;

    public Guid RestauranteId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Dinheiro Preco { get; private set; } = null!;
    public bool Excluido { get; private set; }

    public DateTime AtualizadoEm { get; private set; }

    private ProdutoCatalogo()
    {
    }

    public static ProdutoCatalogo Criar(Guid produtoId, Guid restauranteId, string nome, Dinheiro preco, bool excluido, DateTime atualizadoEm)
    {
        if (produtoId == Guid.Empty)
            throw new DomainException("O Id do produto é obrigatório.");

        if (restauranteId == Guid.Empty)
            throw new DomainException("O produto precisa pertencer a um restaurante.");

        var produto = new ProdutoCatalogo { Id = produtoId, RestauranteId = restauranteId };
        produto.Aplicar(nome, preco, excluido, atualizadoEm);
        return produto;
    }

    public bool AplicarSeForMaisNovo(string nome, Dinheiro preco, bool excluido, DateTime ocorridoEm)
    {
        if (ocorridoEm <= AtualizadoEm)
            return false;

        Aplicar(nome, preco, excluido, ocorridoEm);
        return true;
    }

    private void Aplicar(string nome, Dinheiro preco, bool excluido, DateTime atualizadoEm)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do produto é obrigatório.");

        Nome = nome.Trim();
        Preco = preco;
        Excluido = excluido;
        AtualizadoEm = atualizadoEm;
    }
}
