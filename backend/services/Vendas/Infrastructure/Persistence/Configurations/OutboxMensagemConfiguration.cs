using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Core.Mensageria;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Configurations;

public class OutboxMensagemConfiguration : IEntityTypeConfiguration<OutboxMensagem>
{
    public void Configure(EntityTypeBuilder<OutboxMensagem> builder)
    {
        builder.ToTable("OutboxMensagens");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Fila)
            .HasMaxLength(OutboxMensagem.FilaMaxLength)
            .IsRequired();

        builder.Property(m => m.Tipo)
            .HasMaxLength(OutboxMensagem.TipoMaxLength)
            .IsRequired();

        builder.Property(m => m.Conteudo).IsRequired();

        builder.Property(m => m.Tentativas).IsRequired();
        builder.Property(m => m.CriadoEm).IsRequired();
        builder.Property(m => m.ProximaTentativaEm).IsRequired();
        builder.Property(m => m.ProcessadoEm);

        builder.Property(m => m.UltimoErro)
            .HasMaxLength(OutboxMensagem.UltimoErroMaxLength);
    }
}
