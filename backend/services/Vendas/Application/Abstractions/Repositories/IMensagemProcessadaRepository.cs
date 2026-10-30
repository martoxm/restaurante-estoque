using RestauranteEstoque.Core.Mensageria;

namespace RestauranteEstoque.Vendas.Application.Abstractions.Repositories;

public interface IMensagemProcessadaRepository
{
    Task<bool> ExisteAsync(Guid mensagemId, CancellationToken cancellationToken);

    void Adicionar(MensagemProcessada mensagem);
}
