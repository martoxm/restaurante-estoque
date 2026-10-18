using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Infrastructure.Time;

public class Relogio : IRelogio
{
    private readonly TimeProvider _timeProvider;

    public Relogio(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public DateTime AgoraUtc => _timeProvider.GetUtcNow().UtcDateTime;
}
