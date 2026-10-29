using RestauranteEstoque.Identidade.Contracts.Common;
using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Application.Abstractions.Repositories;

public interface IRestauranteRepository
{
    Task<Restaurante?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Restaurante> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? nome,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken);

    void Adicionar(Restaurante restaurante);

    void Atualizar(Restaurante restaurante);
}
