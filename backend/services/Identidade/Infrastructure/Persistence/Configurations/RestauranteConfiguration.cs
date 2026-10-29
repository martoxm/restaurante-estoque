using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Infrastructure.Persistence.Configurations;

public class RestauranteConfiguration : IEntityTypeConfiguration<Restaurante>
{
    public void Configure(EntityTypeBuilder<Restaurante> builder)
    {
        builder.ToTable("Restaurantes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(r => r.CriadoEm)
            .IsRequired();
    }
}
