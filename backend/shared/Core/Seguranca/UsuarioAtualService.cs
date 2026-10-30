using RestauranteEstoque.Core.Application.Abstractions;
using Microsoft.AspNetCore.Http;
using RestauranteEstoque.Core.Application.Exceptions;

namespace RestauranteEstoque.Core.Seguranca;

public class UsuarioAtualService : IUsuarioAtualService, IDefinidorRestauranteAtual
{
    public const string RestauranteIdClaimType = "restauranteId";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAtualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private Guid? _restauranteDefinido;

    public void Definir(Guid restauranteId) => _restauranteDefinido = restauranteId;

    public Guid? RestauranteId
    {
        get
        {
            if (_restauranteDefinido is not null)
                return _restauranteDefinido;

            var claimValue = _httpContextAccessor.HttpContext?.User
                .FindFirst(RestauranteIdClaimType)?.Value;

            return Guid.TryParse(claimValue, out var restauranteId) ? restauranteId : null;
        }
    }

    public Guid ObterRestauranteIdObrigatorio() =>
        RestauranteId ?? throw new SessaoInvalidaException();
}
