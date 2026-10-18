using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Produtos;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeProdutoRepository : IProdutoRepository
{
    private readonly TestDbContext _context;

    public FakeProdutoRepository(TestDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Produtos
            .Include(produto => produto.Categoria)
            .FirstOrDefaultAsync(produto => produto.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Produto> Itens, int TotalItens)> ListarPaginadoAsync(
        int pagina,
        int tamanhoPagina,
        string? nome,
        Guid? categoriaId,
        OrdenacaoProduto ordenarPor,
        DirecaoOrdenacao direcaoOrdenacao,
        CancellationToken cancellationToken)
    {
        IQueryable<Produto> query = _context.Produtos
            .Include(produto => produto.Categoria);

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(produto => produto.Nome.Contains(nome));

        if (categoriaId.HasValue)
            query = query.Where(produto => produto.CategoriaId == categoriaId.Value);

        var totalItens = await query.CountAsync(cancellationToken);

        var itens = await AplicarOrdenacao(query, ordenarPor, direcaoOrdenacao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (itens, totalItens);
    }

    public void Adicionar(Produto produto)
    {
        _context.Produtos.Add(produto);
    }

    public void Atualizar(Produto produto)
    {
        _context.Produtos.Update(produto);
    }

    public void Remover(Produto produto)
    {
        _context.Produtos.Remove(produto);
    }

    private static IQueryable<Produto> AplicarOrdenacao(IQueryable<Produto> query, OrdenacaoProduto ordenarPor, DirecaoOrdenacao direcaoOrdenacao)
    {
        return (ordenarPor, direcaoOrdenacao) switch
        {
            (OrdenacaoProduto.Nome, DirecaoOrdenacao.Desc) => query.OrderByDescending(produto => produto.Nome),
            (OrdenacaoProduto.Preco, DirecaoOrdenacao.Asc) => query.OrderBy(produto => produto.Preco),
            (OrdenacaoProduto.Preco, DirecaoOrdenacao.Desc) => query.OrderByDescending(produto => produto.Preco),
            (OrdenacaoProduto.QuantidadeEmEstoque, DirecaoOrdenacao.Asc) => query.OrderBy(produto => produto.QuantidadeEmEstoque),
            (OrdenacaoProduto.QuantidadeEmEstoque, DirecaoOrdenacao.Desc) => query.OrderByDescending(produto => produto.QuantidadeEmEstoque),
            _ => query.OrderBy(produto => produto.Nome)
        };
    }
}
