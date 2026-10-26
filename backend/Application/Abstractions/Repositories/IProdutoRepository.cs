using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Produtos;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Abstractions.Repositories;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Produto> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? nome,
        Guid? categoriaId,
        OrdenacaoProduto ordenarPor,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken);

    void Adicionar(Produto produto);

    void Atualizar(Produto produto);

    void Remover(Produto produto);

    Task<bool> ExisteParaCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken);

    Task<int> ContarPorCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken);
}
