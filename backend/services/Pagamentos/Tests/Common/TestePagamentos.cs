using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Services;
using RestauranteEstoque.Pagamentos.Domain.Entities;
using RestauranteEstoque.Pagamentos.Infrastructure.Persistence;

namespace RestauranteEstoque.Pagamentos.Tests.Common;

public static class TestePagamentos
{
    public static readonly Guid RestauranteId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly DateTime Agora = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    public static AppDbContext CriarContexto(IUsuarioAtualService? usuarioAtual = null, string? nomeBanco = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nomeBanco ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, usuarioAtual ?? new FakeUsuarioAtualService { RestauranteId = RestauranteId });
    }

    public static Pagamento NovoPagamento(string token = "tok_aprovado", Guid? restauranteId = null, string? chave = null) =>
        Pagamento.Criar(restauranteId ?? RestauranteId, Guid.NewGuid(), Dinheiro.Criar(50m), token, chave ?? Guid.NewGuid().ToString(), Agora);
}

public class FakeUsuarioAtualService : IUsuarioAtualService, IDefinidorRestauranteAtual
{
    public void Definir(Guid restauranteId) => RestauranteId = restauranteId;

    public Guid? RestauranteId { get; set; }

    public Guid ObterRestauranteIdObrigatorio() =>
        RestauranteId ?? throw new SessaoInvalidaException();
}

public class FakeRelogio : IRelogio
{
    public DateTime AgoraUtc { get; set; } = TestePagamentos.Agora;
}
