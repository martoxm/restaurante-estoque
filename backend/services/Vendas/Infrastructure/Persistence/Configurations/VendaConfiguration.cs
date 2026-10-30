using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Configurations;

public class VendaConfiguration : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(venda => venda.Id);

        builder.Property(venda => venda.RestauranteId);

        builder.Property(venda => venda.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(venda => venda.CriadoEm)
            .IsRequired();

        builder.Ignore(venda => venda.Total);

        builder.HasMany(venda => venda.Itens)
            .WithOne()
            .HasForeignKey(item => item.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(venda => venda.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
