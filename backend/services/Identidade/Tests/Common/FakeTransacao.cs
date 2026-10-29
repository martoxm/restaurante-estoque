using RestauranteEstoque.Identidade.Application.Abstractions.Services;

namespace RestauranteEstoque.Identidade.Tests.Common;

public sealed class FakeTransacao : ITransacao
{
    public bool Confirmada { get; private set; }
    public bool Descartada { get; private set; }

    public Task ConfirmarAsync(CancellationToken cancellationToken)
    {
        Confirmada = true;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Descartada = true;
        return ValueTask.CompletedTask;
    }
}
