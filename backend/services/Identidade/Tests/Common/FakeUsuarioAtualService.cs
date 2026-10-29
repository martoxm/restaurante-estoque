using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Core.Application.Exceptions;

namespace RestauranteEstoque.Identidade.Tests.Common;

public class FakeUsuarioAtualService : IUsuarioAtualService
{
    public Guid? RestauranteId { get; set; }

    public Guid ObterRestauranteIdObrigatorio() =>
        RestauranteId ?? throw new SessaoInvalidaException();
}
