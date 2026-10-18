using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.ValueObjects;

namespace RestauranteEstoque.Infrastructure.Persistence.Configurations;

public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
{
    public void Configure(EntityTypeBuilder<Fornecedor> builder)
    {
        builder.ToTable("Fornecedores");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.Telefone)
            .HasMaxLength(20);

        builder.Property(s => s.Email)
            .HasConversion(email => email.Valor, valor => Email.Criar(valor))
            .HasMaxLength(Email.TamanhoMaximo);

        builder.Property(s => s.RestauranteId);

        builder.Property(s => s.CriadoEm)
            .IsRequired();
    }
}
