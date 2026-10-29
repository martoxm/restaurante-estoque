using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Abstractions.Repositories;

public interface IMovimentacaoEstoqueRepository
{
    void Adicionar(MovimentacaoEstoque movimentacao);

    Task<bool> ExisteParaProdutoAsync(Guid produtoId, CancellationToken cancellationToken);
}
