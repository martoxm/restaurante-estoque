using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Vendas.Domain.Entities;

public class ItemVenda : Entity
{
    public Guid VendaId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public Dinheiro PrecoUnitario { get; private set; } = Dinheiro.Zero;
    public Quantidade Quantidade { get; private set; } = Quantidade.Zero;

    public Dinheiro Subtotal => Dinheiro.Criar(PrecoUnitario.Valor * Quantidade.Valor);

    private ItemVenda()
    {
    }

    internal static ItemVenda Criar(Guid vendaId, Guid produtoId, string nomeProduto, Dinheiro precoUnitario, Quantidade quantidade)
    {
        if (produtoId == Guid.Empty)
            throw new DomainException("O item da venda precisa de um produto.");

        if (string.IsNullOrWhiteSpace(nomeProduto))
            throw new DomainException("O nome do produto é obrigatório no item da venda.");

        if (quantidade.EhZero)
            throw new DomainException("A quantidade do item precisa ser maior que zero.");

        return new ItemVenda
        {
            VendaId = vendaId,
            ProdutoId = produtoId,
            NomeProduto = nomeProduto.Trim(),
            PrecoUnitario = precoUnitario,
            Quantidade = quantidade
        };
    }
}
