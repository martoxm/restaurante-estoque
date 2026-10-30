using MediatR;
using RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;

namespace RestauranteEstoque.Api.Workers;

public sealed class DevolucaoEstoqueConsumidor : ConsumidorFilaBase<DevolverEstoqueMensagem>
{
    private readonly ILogger<DevolucaoEstoqueConsumidor> _logger;

    public DevolucaoEstoqueConsumidor(ConexaoRabbitMq conexao, IServiceScopeFactory scopeFactory, ILogger<DevolucaoEstoqueConsumidor> logger)
        : base(conexao, scopeFactory, logger)
    {
        _logger = logger;
    }

    protected override string Fila => Filas.DevolverEstoque;

    protected override async Task<bool> ProcessarAsync(DevolverEstoqueMensagem mensagem, Guid mensagemId, IServiceProvider servicos, CancellationToken cancellationToken)
    {
        if (mensagemId == Guid.Empty)
        {
            _logger.LogError("Devolução da venda {VendaId} chegou sem MessageId.", mensagem.VendaId);
            return false;
        }

        var sender = servicos.GetRequiredService<ISender>();
        await sender.Send(
            new DevolverEstoqueVendaCommand(mensagemId, mensagem.VendaId, mensagem.RestauranteId, mensagem.Itens, mensagem.Motivo),
            cancellationToken);

        return true;
    }
}
