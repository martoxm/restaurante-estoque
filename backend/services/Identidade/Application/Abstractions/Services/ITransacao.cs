namespace RestauranteEstoque.Identidade.Application.Abstractions.Services;

public interface ITransacao : IAsyncDisposable
{
    Task ConfirmarAsync(CancellationToken cancellationToken);
}
