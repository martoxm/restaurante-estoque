using RestauranteEstoque.Vendas.Contracts.Common;
using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Application.Abstractions.Repositories;

public interface IVendaRepository
{
    Task<Venda?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Venda> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken);

    void Adicionar(Venda venda);

    void Atualizar(Venda venda);
}
