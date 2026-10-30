using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Application.Abstractions.Repositories;

public interface IProdutoCatalogoRepository
{
    Task<ProdutoCatalogo?> ObterPorIdAsync(Guid produtoId, CancellationToken cancellationToken);

    void Adicionar(ProdutoCatalogo produto);

    void Atualizar(ProdutoCatalogo produto);
}
