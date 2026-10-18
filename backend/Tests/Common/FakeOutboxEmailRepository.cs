using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Common;

public class FakeOutboxEmailRepository : IOutboxEmailRepository
{
    public List<OutboxEmail> Emails { get; } = new();

    public void Adicionar(OutboxEmail email) => Emails.Add(email);

    public void Atualizar(OutboxEmail email)
    {
    }

    public Task<IReadOnlyList<OutboxEmail>> ListarPendentesAsync(int quantidade, DateTime agora, CancellationToken cancellationToken)
    {
        IReadOnlyList<OutboxEmail> pendentes = Emails
            .Where(email => email.ProcessadoEm == null && email.ProximaTentativaEm <= agora)
            .OrderBy(email => email.CriadoEm)
            .Take(quantidade)
            .ToList();

        return Task.FromResult(pendentes);
    }
}
