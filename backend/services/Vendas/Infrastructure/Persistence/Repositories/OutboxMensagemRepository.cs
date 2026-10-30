using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Vendas.Application.Abstractions.Repositories;
using RestauranteEstoque.Core.Mensageria;

namespace RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;

public class OutboxMensagemRepository : IOutboxMensagemRepository
{
    private readonly AppDbContext _context;

    public OutboxMensagemRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Adicionar(OutboxMensagem mensagem) => _context.OutboxMensagens.Add(mensagem);

    public void Atualizar(OutboxMensagem mensagem) => _context.OutboxMensagens.Update(mensagem);

    public async Task<IReadOnlyList<OutboxMensagem>> ListarPendentesAsync(int quantidade, DateTime agora, CancellationToken cancellationToken)
    {
        return await _context.OutboxMensagens
            .Where(mensagem => mensagem.ProcessadoEm == null && mensagem.ProximaTentativaEm <= agora)
            .OrderBy(mensagem => mensagem.CriadoEm)
            .Take(quantidade)
            .ToListAsync(cancellationToken);
    }
}
