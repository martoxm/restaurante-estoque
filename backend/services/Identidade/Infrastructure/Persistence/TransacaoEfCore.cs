using Microsoft.EntityFrameworkCore.Storage;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;

namespace RestauranteEstoque.Identidade.Infrastructure.Persistence;

public sealed class TransacaoEfCore : ITransacao
{
    private readonly IDbContextTransaction _transacao;

    public TransacaoEfCore(IDbContextTransaction transacao)
    {
        _transacao = transacao;
    }

    public Task ConfirmarAsync(CancellationToken cancellationToken) => _transacao.CommitAsync(cancellationToken);

    public ValueTask DisposeAsync() => _transacao.DisposeAsync();
}
