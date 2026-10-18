using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class FornecedorRepository : IFornecedorRepository
{
    private readonly AppDbContext _context;

    public FornecedorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Fornecedor?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Fornecedores
            .FirstOrDefaultAsync(fornecedor => fornecedor.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Fornecedor> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? nome,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken)
    {
        IQueryable<Fornecedor> query = _context.Fornecedores;

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(fornecedor => fornecedor.Nome.Contains(nome));

        var totalItens = await query.CountAsync(cancellationToken);

        query = direcaoOrdenacao == DirecaoOrdenacao.Desc
            ? query.OrderByDescending(fornecedor => fornecedor.Nome)
            : query.OrderBy(fornecedor => fornecedor.Nome);

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (itens, totalItens);
    }

    public void Adicionar(Fornecedor fornecedor) => _context.Fornecedores.Add(fornecedor);

    public void Atualizar(Fornecedor fornecedor) => _context.Fornecedores.Update(fornecedor);
    
    public void Remover(Fornecedor fornecedor) => _context.Fornecedores.Remove(fornecedor);
    
}