using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class MovimentacaoEstoqueRepository : IMovimentacaoEstoqueRepository
{
    private readonly AppDbContext _context;

    public MovimentacaoEstoqueRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Adicionar(MovimentacaoEstoque movimentacao) => _context.MovimentacoesEstoque.Add(movimentacao);

    public async Task<bool> ExisteParaProdutoAsync(Guid produtoId, CancellationToken cancellationToken)
    {
        return await _context.MovimentacoesEstoque.AnyAsync(
            movimentacao => movimentacao.ProdutoId == produtoId,
            cancellationToken);
    }
}
