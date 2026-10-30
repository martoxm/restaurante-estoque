using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Vendas.Application.Abstractions.Services;
using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly IUsuarioAtualService _usuarioAtualService;

    public AppDbContext(DbContextOptions<AppDbContext> options, IUsuarioAtualService usuarioAtualService)
        : base(options)
    {
        _usuarioAtualService = usuarioAtualService;
    }

    private Guid? RestauranteIdAtual => _usuarioAtualService.RestauranteId;

    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<OutboxMensagem> OutboxMensagens => Set<OutboxMensagem>();
    public DbSet<MensagemProcessada> MensagensProcessadas => Set<MensagemProcessada>();
    public DbSet<ProdutoCatalogo> ProdutosCatalogo => Set<ProdutoCatalogo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("vendas");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Venda>().HasQueryFilter(venda => venda.RestauranteId == RestauranteIdAtual);

        modelBuilder.Entity<ProdutoCatalogo>().HasQueryFilter(produto => produto.RestauranteId == RestauranteIdAtual);
    }
}
