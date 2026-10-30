using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Vendas.Application.Abstractions.Repositories;
using RestauranteEstoque.Vendas.Contracts.Common;
using RestauranteEstoque.Vendas.Domain.Entities;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly AppDbContext _context;

    public VendaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Venda?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Vendas
            .Include(venda => venda.Itens)
            .FirstOrDefaultAsync(venda => venda.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Venda> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken)
    {
        var query = _context.Vendas
            .Include(venda => venda.Itens)
            .AsQueryable();

        var totalItens = await query.CountAsync(cancellationToken);

        query = direcaoOrdenacao == DirecaoOrdenacao.Desc
            ? query.OrderByDescending(venda => venda.CriadoEm)
            : query.OrderBy(venda => venda.CriadoEm);

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return (itens, totalItens);
    }

    public void Adicionar(Venda venda) => _context.Vendas.Add(venda);

    public void Atualizar(Venda venda) => _context.Entry(venda).State = EntityState.Modified;
}
