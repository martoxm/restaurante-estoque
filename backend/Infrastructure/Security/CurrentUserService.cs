using Microsoft.AspNetCore.Http;
using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Infrastructure.Security;

public class CurrentUserService : ICurrentUserService
{
    public const string RestaurantIdClaimType = "restaurantId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? RestaurantId
    {
        get
        {
            var claimValue = _httpContextAccessor.HttpContext?.User
                .FindFirst(RestaurantIdClaimType)?.Value;

            return Guid.TryParse(claimValue, out var restaurantId) ? restaurantId : null;
        }
    }
}
