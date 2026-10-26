using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Descricao)
            .HasMaxLength(500);

        builder.Property(p => p.Preco)
            .HasConversion(preco => preco.Valor, valor => Dinheiro.Criar(valor))
            .HasPrecision(10, 2);

        builder.Property(p => p.QuantidadeEmEstoque)
            .HasConversion(quantidade => quantidade.Valor, valor => Quantidade.Criar(valor))
            .IsRequired();

        builder.Property(p => p.RestauranteId);

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaId)
            .IsRequired();
    }
}
