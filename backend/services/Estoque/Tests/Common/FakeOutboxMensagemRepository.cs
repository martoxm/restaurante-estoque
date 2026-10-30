using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Core.Mensageria;

namespace RestauranteEstoque.Tests.Common;

public class FakeOutboxMensagemRepository : IOutboxMensagemRepository
{
    public List<OutboxMensagem> Mensagens { get; } = new();

    public void Adicionar(OutboxMensagem mensagem) => Mensagens.Add(mensagem);

    public void Atualizar(OutboxMensagem mensagem)
    {
    }

    public Task<IReadOnlyList<OutboxMensagem>> ListarPendentesAsync(int quantidade, DateTime agora, CancellationToken cancellationToken)
    {
        IReadOnlyList<OutboxMensagem> pendentes = Mensagens
            .Where(mensagem => mensagem.ProcessadoEm == null && mensagem.ProximaTentativaEm <= agora)
            .OrderBy(mensagem => mensagem.CriadoEm)
            .Take(quantidade)
            .ToList();

        return Task.FromResult(pendentes);
    }
}
