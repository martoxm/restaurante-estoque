using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Core.Mensageria;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Configurations;

public class MensagemProcessadaConfiguration : IEntityTypeConfiguration<MensagemProcessada>
{
    public void Configure(EntityTypeBuilder<MensagemProcessada> builder)
    {
        builder.ToTable("MensagensProcessadas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Fila).HasMaxLength(OutboxMensagem.FilaMaxLength).IsRequired();
        builder.Property(m => m.ProcessadoEm).IsRequired();
    }
}
