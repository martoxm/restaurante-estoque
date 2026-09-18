using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Tests.Common;

public class FakeClock : IClock
{
    public DateTime UtcNow { get; set; } = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
}
