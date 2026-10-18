using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Configurations;

public class OutboxEmailConfiguration : IEntityTypeConfiguration<OutboxEmail>
{
    public void Configure(EntityTypeBuilder<OutboxEmail> builder)
    {
        builder.ToTable("OutboxEmails");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RestauranteId).IsRequired();

        builder.Property(e => e.Assunto)
            .HasMaxLength(OutboxEmail.AssuntoMaxLength)
            .IsRequired();

        builder.Property(e => e.CorpoHtml).IsRequired();

        builder.Property(e => e.Tentativas).IsRequired();
        builder.Property(e => e.CriadoEm).IsRequired();
        builder.Property(e => e.ProximaTentativaEm).IsRequired();
        builder.Property(e => e.ProcessadoEm);

        builder.Property(e => e.UltimoErro)
            .HasMaxLength(OutboxEmail.UltimoErroMaxLength);
    }
}
