using RestauranteEstoque.Pagamentos.Domain.Entities;

namespace RestauranteEstoque.Pagamentos.Application.Abstractions.Repositories;

public interface IPagamentoRepository
{
    Task<Pagamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Pagamento?> ObterPorVendaAsync(Guid vendaId, CancellationToken cancellationToken);
    Task<Pagamento?> ObterPorChaveIdempotenciaAsync(string chaveIdempotencia, CancellationToken cancellationToken);

    Task<IReadOnlyList<PagamentoPendente>> ListarPendentesDeTodosAsync(int limite, CancellationToken cancellationToken);

    void Adicionar(Pagamento pagamento);
    void Atualizar(Pagamento pagamento);
}

public record PagamentoPendente(Guid PagamentoId, Guid RestauranteId);
