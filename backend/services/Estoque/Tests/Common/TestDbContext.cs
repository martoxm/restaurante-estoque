using RestauranteEstoque.Core.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Core.Domain.ValueObjects;

namespace RestauranteEstoque.Tests.Common;

public class TestDbContext : DbContext, IApplicationDbContext
{
    private readonly IUsuarioAtualService _usuarioAtualService;

    public TestDbContext(DbContextOptions<TestDbContext> options, IUsuarioAtualService usuarioAtualService)
        : base(options)
    {
        _usuarioAtualService = usuarioAtualService;
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    private Guid? RestauranteIdAtual => _usuarioAtualService.RestauranteId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>().HasQueryFilter(produto => produto.RestauranteId == RestauranteIdAtual);
        modelBuilder.Entity<Categoria>().HasQueryFilter(categoria => categoria.RestauranteId == RestauranteIdAtual);
        modelBuilder.Entity<Fornecedor>().HasQueryFilter(fornecedor => fornecedor.RestauranteId == RestauranteIdAtual);
        modelBuilder.Entity<MovimentacaoEstoque>().HasQueryFilter(movimentacao => movimentacao.RestauranteId == RestauranteIdAtual);

        modelBuilder.Entity<Produto>().Property(produto => produto.Preco)
            .HasConversion(preco => preco.Valor, valor => Dinheiro.Criar(valor));
        modelBuilder.Entity<Produto>().Property(produto => produto.QuantidadeEmEstoque)
            .HasConversion(quantidade => quantidade.Valor, valor => Quantidade.Criar(valor));
        modelBuilder.Entity<MovimentacaoEstoque>().Property(movimentacao => movimentacao.Quantidade)
            .HasConversion(quantidade => quantidade.Valor, valor => Quantidade.Criar(valor));
        modelBuilder.Entity<Fornecedor>().Property(fornecedor => fornecedor.Email)
            .HasConversion(email => email.Valor, valor => Email.Criar(valor));
    }
}

public static class TestDbContextFactory
{
    public static TestDbContext Create(IUsuarioAtualService? usuarioAtualService = null)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options, usuarioAtualService ?? new FakeUsuarioAtualService { RestauranteId = DadosTeste.RestauranteId });
    }
}
