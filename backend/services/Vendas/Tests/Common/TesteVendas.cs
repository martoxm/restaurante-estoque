using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Vendas.Application.Abstractions.Services;
using RestauranteEstoque.Vendas.Domain.Entities;
using RestauranteEstoque.Vendas.Infrastructure.Persistence;

namespace RestauranteEstoque.Vendas.Tests.Common;

public static class TesteVendas
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

    public static async Task<ProdutoCatalogo> AdicionarProdutoAsync(AppDbContext context, string nome, decimal preco, Guid? restauranteId = null, bool excluido = false)
    {
        var produto = ProdutoCatalogo.Criar(Guid.NewGuid(), restauranteId ?? RestauranteId, nome, Dinheiro.Criar(preco), excluido, Agora);
        context.ProdutosCatalogo.Add(produto);
        await context.SaveChangesAsync();
        return produto;
    }
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
    public DateTime AgoraUtc { get; set; } = TesteVendas.Agora;
}
