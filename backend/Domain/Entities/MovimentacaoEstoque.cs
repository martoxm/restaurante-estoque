using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Domain.Entities;

public class MovimentacaoEstoque : Entity
{
    public Guid ProdutoId { get; private set; }
    public Produto? Produto { get; private set; }
    public TipoMovimentacaoEstoque Tipo { get; private set; }
    public Quantidade Quantidade { get; private set; } = Quantidade.Zero;
    public DateTime DataMovimentacao { get; private set; }
    public string? Observacoes { get; private set; }

    public Guid RestauranteId { get; private set; }

    private MovimentacaoEstoque()
    {
    }

    internal static MovimentacaoEstoque Criar(Guid restauranteId, Guid produtoId, TipoMovimentacaoEstoque tipo, Quantidade quantidade, DateTime dataMovimentacao, string? observacoes = null)
    {
        if (restauranteId == Guid.Empty)
            throw new DomainException("A movimentação precisa pertencer a um restaurante.");

        ValidarProdutoId(produtoId);
        ValidarQuantidade(quantidade);

        return new MovimentacaoEstoque
        {
            ProdutoId = produtoId,
            Tipo = tipo,
            Quantidade = quantidade,
            Observacoes = observacoes?.Trim(),
            DataMovimentacao = dataMovimentacao,
            RestauranteId = restauranteId
        };
    }

    private static void ValidarProdutoId(Guid produtoId)
    {
        if (produtoId == Guid.Empty)
            throw new DomainException("A movimentação precisa estar vinculada a um produto.");
    }

    private static void ValidarQuantidade(Quantidade quantidade)
    {
        if (quantidade.EhZero)
            throw new DomainException("A quantidade movimentada deve ser maior que zero.");
    }
}
