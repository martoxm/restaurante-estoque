using MediatR;
using RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;

namespace RestauranteEstoque.Api.Workers;

public sealed class BaixaEstoqueConsumidor : ConsumidorFilaBase<BaixarEstoqueMensagem>
{
    private readonly ILogger<BaixaEstoqueConsumidor> _logger;

    public BaixaEstoqueConsumidor(ConexaoRabbitMq conexao, IServiceScopeFactory scopeFactory, ILogger<BaixaEstoqueConsumidor> logger)
        : base(conexao, scopeFactory, logger)
    {
        _logger = logger;
    }

    protected override string Fila => Filas.BaixarEstoque;

    protected override async Task<bool> ProcessarAsync(BaixarEstoqueMensagem mensagem, Guid mensagemId, IServiceProvider servicos, CancellationToken cancellationToken)
    {
        if (mensagemId == Guid.Empty)
        {
            _logger.LogError("Pedido de baixa da venda {VendaId} chegou sem MessageId.", mensagem.VendaId);
            return false;
        }

        var sender = servicos.GetRequiredService<ISender>();
        await sender.Send(new BaixarEstoqueVendaCommand(mensagemId, mensagem.VendaId, mensagem.RestauranteId, mensagem.Itens), cancellationToken);

        return true;
    }
}
