using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeCategoriaRepository : ICategoriaRepository
{
    private readonly TestDbContext _context;

    public FakeCategoriaRepository(TestDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categorias
            .Include(categoria => categoria.Produtos)
            .FirstOrDefaultAsync(categoria => categoria.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Categoria> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? nome,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken)
    {
        IQueryable<Categoria> query = _context.Categorias
            .Include(categoria => categoria.Produtos);

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(categoria => categoria.Nome.Contains(nome));

        var totalItens = await query.CountAsync(cancellationToken);

        query = direcaoOrdenacao == DirecaoOrdenacao.Desc
            ? query.OrderByDescending(categoria => categoria.Nome)
            : query.OrderBy(categoria => categoria.Nome);

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (itens, totalItens);
    }

    public void Adicionar(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
    }

    public void Atualizar(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
    }

    public void Remover(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
    }
}
