using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Abstractions.Repositories;

public interface IOutboxEmailRepository
{
    void Adicionar(OutboxEmail email);

    void Atualizar(OutboxEmail email);

    Task<IReadOnlyList<OutboxEmail>> ListarPendentesAsync(int quantidade, DateTime agora, CancellationToken cancellationToken);
}
