using RestauranteEstoque.Core.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly IUsuarioAtualService _usuarioAtualService;

    public AppDbContext(DbContextOptions<AppDbContext> options, IUsuarioAtualService usuarioAtualService)
        : base(options)
    {
        _usuarioAtualService = usuarioAtualService;
    }

    private Guid? RestauranteIdAtual => _usuarioAtualService.RestauranteId;

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("estoque");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Produto>().HasQueryFilter(produto => produto.RestauranteId == RestauranteIdAtual);
        modelBuilder.Entity<Categoria>().HasQueryFilter(categoria => categoria.RestauranteId == RestauranteIdAtual);
        modelBuilder.Entity<Fornecedor>().HasQueryFilter(fornecedor => fornecedor.RestauranteId == RestauranteIdAtual);
        modelBuilder.Entity<MovimentacaoEstoque>().HasQueryFilter(movimentacao => movimentacao.RestauranteId == RestauranteIdAtual);
    }
}
