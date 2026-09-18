using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Infrastructure.Time;

public class Clock : IClock
{
    private readonly TimeProvider _timeProvider;

    public Clock(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;
}
