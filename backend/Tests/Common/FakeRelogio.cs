using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Tests.Common;

public class FakeRelogio : IRelogio
{
    public DateTime AgoraUtc { get; set; } = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
}
