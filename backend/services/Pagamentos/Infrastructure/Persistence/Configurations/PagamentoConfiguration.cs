using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestauranteEstoque.Core.Domain.ValueObjects;
using RestauranteEstoque.Pagamentos.Domain.Entities;

namespace RestauranteEstoque.Pagamentos.Infrastructure.Persistence.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamentos");

        builder.HasKey(pagamento => pagamento.Id);

        builder.Property(pagamento => pagamento.RestauranteId);
        builder.Property(pagamento => pagamento.VendaId);

        builder.Property(pagamento => pagamento.Valor)
            .HasConversion(valor => valor.Valor, valor => Dinheiro.Criar(valor))
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(pagamento => pagamento.TokenCartao)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pagamento => pagamento.ChaveIdempotencia)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(pagamento => pagamento.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(pagamento => pagamento.CriadoEm).IsRequired();
        builder.Property(pagamento => pagamento.ProcessadoEm);
        builder.Property(pagamento => pagamento.IdTransacaoGateway).HasMaxLength(100);
        builder.Property(pagamento => pagamento.MotivoRecusa).HasMaxLength(300);
        builder.Property(pagamento => pagamento.Tentativas).IsRequired();

        builder.HasIndex(pagamento => new { pagamento.RestauranteId, pagamento.ChaveIdempotencia })
            .IsUnique();
    }
}
