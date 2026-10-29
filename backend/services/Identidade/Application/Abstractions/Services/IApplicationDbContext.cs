namespace RestauranteEstoque.Identidade.Application.Abstractions.Services;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransacao> IniciarTransacaoAsync(CancellationToken cancellationToken);
}
