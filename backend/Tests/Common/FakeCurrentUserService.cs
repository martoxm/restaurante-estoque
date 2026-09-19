using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Tests.Common;

public class FakeCurrentUserService : ICurrentUserService
{
    public Guid? RestaurantId { get; set; }
}
