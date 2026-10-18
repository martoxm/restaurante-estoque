using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;

namespace RestauranteEstoque.Tests.Common;

public class FakeUsuarioAtualService : IUsuarioAtualService
{
    public Guid? RestauranteId { get; set; }

    public Guid ObterRestauranteIdObrigatorio() =>
        RestauranteId ?? throw new SessaoInvalidaException();
}
