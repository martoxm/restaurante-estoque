using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Vendas.Application.Abstractions.Repositories;
using RestauranteEstoque.Core.Mensageria;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;

public class MensagemProcessadaRepository : IMensagemProcessadaRepository
{
    private readonly AppDbContext _context;

    public MensagemProcessadaRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteAsync(Guid mensagemId, CancellationToken cancellationToken) =>
        _context.MensagensProcessadas.AnyAsync(mensagem => mensagem.Id == mensagemId, cancellationToken);

    public void Adicionar(MensagemProcessada mensagem) => _context.MensagensProcessadas.Add(mensagem);
}
