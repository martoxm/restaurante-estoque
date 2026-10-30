using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Configurations;

public class ItemVendaConfiguration : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("ItensVenda");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.ProdutoId);

        builder.Property(item => item.NomeProduto)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(item => item.PrecoUnitario)
            .HasConversion(preco => preco.Valor, valor => Dinheiro.Criar(valor))
            .HasPrecision(10, 2);

        builder.Property(item => item.Quantidade)
            .HasConversion(quantidade => quantidade.Valor, valor => Quantidade.Criar(valor))
            .IsRequired();

        builder.Ignore(item => item.Subtotal);
    }
}
