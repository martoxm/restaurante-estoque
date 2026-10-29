using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categorias
            .FirstOrDefaultAsync(categoria => categoria.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<(Categoria Categoria, int QuantidadeProdutos)> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? nome,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken)
    {
        var query = _context.Categorias.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(categoria => categoria.Nome.Contains(nome));

        var totalItens = await query.CountAsync(cancellationToken);

        query = direcaoOrdenacao == DirecaoOrdenacao.Desc
            ? query.OrderByDescending(categoria => categoria.Nome)
            : query.OrderBy(categoria => categoria.Nome);

        var linhas = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .Select(categoria => new
            {
                Categoria = categoria,
                QuantidadeProdutos = _context.Produtos.Count(produto => produto.CategoriaId == categoria.Id)
            })
            .ToListAsync(cancellationToken);

        var itens = linhas
            .Select(linha => (linha.Categoria, linha.QuantidadeProdutos))
            .ToList();

        return (itens, totalItens);
    }

    public void Adicionar(Categoria categoria) => _context.Categorias.Add(categoria);

    public void Atualizar(Categoria categoria) => _context.Categorias.Update(categoria);

    public void Remover(Categoria categoria) => _context.Categorias.Remove(categoria);
}
