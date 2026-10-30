using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Configurations;

public class ProdutoCatalogoConfiguration : IEntityTypeConfiguration<ProdutoCatalogo>
{
    public void Configure(EntityTypeBuilder<ProdutoCatalogo> builder)
    {
        builder.ToTable("ProdutosCatalogo");

        builder.HasKey(produto => produto.Id);

        builder.Property(produto => produto.Id).ValueGeneratedNever();

        builder.Property(produto => produto.RestauranteId).IsRequired();
        builder.Property(produto => produto.Nome).HasMaxLength(ProdutoCatalogo.NomeMaxLength).IsRequired();

        builder.Property(produto => produto.Preco)
            .HasConversion(preco => preco.Valor, valor => Dinheiro.Criar(valor))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(produto => produto.Excluido).IsRequired();
        builder.Property(produto => produto.AtualizadoEm).IsRequired();
    }
}
