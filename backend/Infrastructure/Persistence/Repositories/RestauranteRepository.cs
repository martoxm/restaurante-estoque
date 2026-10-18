using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class RestauranteRepository : IRestauranteRepository
{
    private readonly AppDbContext _context;

    public RestauranteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Restaurante?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Restaurantes
            .FirstOrDefaultAsync(restaurante => restaurante.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Restaurante> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? nome,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken)
    {
        var query = _context.Restaurantes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(restaurante => restaurante.Nome.Contains(nome));

        var totalItens = await query.CountAsync(cancellationToken);

        query = direcaoOrdenacao == DirecaoOrdenacao.Desc
            ? query.OrderByDescending(restaurante => restaurante.Nome)
            : query.OrderBy(restaurante => restaurante.Nome);

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (itens, totalItens);
    }

    public void Adicionar(Restaurante restaurante) => _context.Restaurantes.Add(restaurante);

    public void Atualizar(Restaurante restaurante) => _context.Restaurantes.Update(restaurante);
}
  
