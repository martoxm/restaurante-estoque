using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Infrastructure.Persistence.Repositories;

public class OutboxEmailRepository : IOutboxEmailRepository
{
    private readonly AppDbContext _context;

    public OutboxEmailRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Adicionar(OutboxEmail email) => _context.OutboxEmails.Add(email);

    public void Atualizar(OutboxEmail email) => _context.OutboxEmails.Update(email);

    public async Task<IReadOnlyList<OutboxEmail>> ListarPendentesAsync(int quantidade, DateTime agora, CancellationToken cancellationToken)
    {
        return await _context.OutboxEmails
            .Where(email => email.ProcessadoEm == null && email.ProximaTentativaEm <= agora)
            .OrderBy(email => email.CriadoEm)
            .Take(quantidade)
            .ToListAsync(cancellationToken);
    }
}
