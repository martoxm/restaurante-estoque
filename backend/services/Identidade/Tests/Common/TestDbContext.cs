using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Tests.Common;

public class TestDbContext : DbContext, IApplicationDbContext
{
    private readonly IUsuarioAtualService _usuarioAtualService;

    public TestDbContext(DbContextOptions<TestDbContext> options, IUsuarioAtualService usuarioAtualService)
        : base(options)
    {
        _usuarioAtualService = usuarioAtualService;
    }

    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    private Guid? RestauranteIdAtual => _usuarioAtualService.RestauranteId;

    public FakeTransacao? UltimaTransacao { get; private set; }

    public Task<ITransacao> IniciarTransacaoAsync(CancellationToken cancellationToken)
    {
        UltimaTransacao = new FakeTransacao();
        return Task.FromResult<ITransacao>(UltimaTransacao);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurante>().HasQueryFilter(restaurante => restaurante.Id == RestauranteIdAtual);
    }
}

public static class TestDbContextFactory
{
    public static TestDbContext Create(IUsuarioAtualService? usuarioAtualService = null)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options, usuarioAtualService ?? new FakeUsuarioAtualService());
    }
}
