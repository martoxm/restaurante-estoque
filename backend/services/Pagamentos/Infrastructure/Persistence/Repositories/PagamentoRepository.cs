using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Repositories;
using RestauranteEstoque.Pagamentos.Domain.Entities;
using RestauranteEstoque.Pagamentos.Domain.Enums;

namespace RestauranteEstoque.Pagamentos.Infrastructure.Persistence.Repositories;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly AppDbContext _context;

    public PagamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pagamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Pagamentos
            .FirstOrDefaultAsync(pagamento => pagamento.Id == id, cancellationToken);
    }

    public async Task<Pagamento?> ObterPorVendaAsync(Guid vendaId, CancellationToken cancellationToken)
    {
        return await _context.Pagamentos
            .FirstOrDefaultAsync(pagamento => pagamento.VendaId == vendaId, cancellationToken);
    }

    public async Task<Pagamento?> ObterPorChaveIdempotenciaAsync(string chaveIdempotencia, CancellationToken cancellationToken)
    {
        var chave = chaveIdempotencia.Trim();
        return await _context.Pagamentos
            .FirstOrDefaultAsync(pagamento => pagamento.ChaveIdempotencia == chave, cancellationToken);
    }

    public async Task<IReadOnlyList<PagamentoPendente>> ListarPendentesDeTodosAsync(int limite, CancellationToken cancellationToken)
    {
        return await _context.Pagamentos
            .IgnoreQueryFilters()
            .Where(pagamento => pagamento.Status == StatusPagamento.Pendente)
            .OrderBy(pagamento => pagamento.CriadoEm)
            .Take(limite)
            .Select(pagamento => new PagamentoPendente(pagamento.Id, pagamento.RestauranteId))
            .ToListAsync(cancellationToken);
    }

    public void Adicionar(Pagamento pagamento) => _context.Pagamentos.Add(pagamento);

    public void Atualizar(Pagamento pagamento) => _context.Entry(pagamento).State = EntityState.Modified;
}
