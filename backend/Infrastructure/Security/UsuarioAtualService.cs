using Microsoft.AspNetCore.Http;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;

namespace RestauranteEstoque.Infrastructure.Security;

public class UsuarioAtualService : IUsuarioAtualService
{
    public const string RestauranteIdClaimType = "restauranteId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? RestauranteId
    {
        get
        {
            var claimValue = _httpContextAccessor.HttpContext?.User
                .FindFirst(RestauranteIdClaimType)?.Value;

            return Guid.TryParse(claimValue, out var restauranteId) ? restauranteId : null;
        }
    }

    public Guid ObterRestauranteIdObrigatorio() =>
        RestauranteId ?? throw new SessaoInvalidaException();
}
