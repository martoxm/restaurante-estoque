using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Core.Application.Exceptions;

namespace RestauranteEstoque.Tests.Common;

public class FakeUsuarioAtualService : IUsuarioAtualService, IDefinidorRestauranteAtual
{
    public Guid? RestauranteId { get; set; }

    public void Definir(Guid restauranteId) => RestauranteId = restauranteId;

    public Guid ObterRestauranteIdObrigatorio() =>
        RestauranteId ?? throw new SessaoInvalidaException();
}
