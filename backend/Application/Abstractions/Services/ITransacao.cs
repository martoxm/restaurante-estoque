namespace RestauranteEstoque.Application.Abstractions.Services;

public interface ITransacao : IAsyncDisposable
{
    Task ConfirmarAsync(CancellationToken cancellationToken);
}
