using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Domain.Services;

public static class MovimentadorEstoque
{
    public static MovimentacaoEstoque Movimentar(
        Produto produto,
        TipoMovimentacaoEstoque tipo,
        Quantidade quantidade,
        DateTime dataMovimentacao,
        string? observacoes = null)
    {
        ArgumentNullException.ThrowIfNull(produto);

        var movimentacao = MovimentacaoEstoque.Criar(
            restauranteId: produto.RestauranteId,
            produtoId: produto.Id,
            tipo: tipo,
            quantidade: quantidade,
            dataMovimentacao: dataMovimentacao,
            observacoes: observacoes);

        switch (tipo)
        {
            case TipoMovimentacaoEstoque.Entrada:
                produto.AumentarEstoque(quantidade);
                break;
            case TipoMovimentacaoEstoque.Saida:
                produto.DiminuirEstoque(quantidade);
                break;
            default:
                throw new DomainException($"Tipo de movimentação '{tipo}' não é suportado.");
        }

        return movimentacao;
    }
}
