using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Services;
using RestauranteEstoque.Pagamentos.Domain.Entities;

namespace RestauranteEstoque.Pagamentos.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly IUsuarioAtualService _usuarioAtualService;

    public AppDbContext(DbContextOptions<AppDbContext> options, IUsuarioAtualService usuarioAtualService)
        : base(options)
    {
        _usuarioAtualService = usuarioAtualService;
    }

    private Guid? RestauranteIdAtual => _usuarioAtualService.RestauranteId;

    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<OutboxMensagem> OutboxMensagens => Set<OutboxMensagem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("pagamentos");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Pagamento>().HasQueryFilter(pagamento => pagamento.RestauranteId == RestauranteIdAtual);
    }
}
