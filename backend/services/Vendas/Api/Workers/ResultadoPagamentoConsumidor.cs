using MediatR;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Vendas.Application.UseCases.Vendas;

namespace RestauranteEstoque.Vendas.Api.Workers;

public sealed class ResultadoPagamentoConsumidor : ConsumidorFilaBase<ResultadoPagamentoMensagem>
{
    private readonly ILogger<ResultadoPagamentoConsumidor> _logger;

    public ResultadoPagamentoConsumidor(ConexaoRabbitMq conexao, IServiceScopeFactory scopeFactory, ILogger<ResultadoPagamentoConsumidor> logger)
        : base(conexao, scopeFactory, logger)
    {
        _logger = logger;
    }

    protected override string Fila => Filas.ResultadoPagamento;

    protected override async Task<bool> ProcessarAsync(ResultadoPagamentoMensagem mensagem, Guid mensagemId, IServiceProvider servicos, CancellationToken cancellationToken)
    {
        if (mensagemId == Guid.Empty)
        {
            _logger.LogError("Resultado do pagamento da venda {VendaId} chegou sem MessageId.", mensagem.VendaId);
            return false;
        }

        var sender = servicos.GetRequiredService<ISender>();
        await sender.Send(new ProcessarResultadoPagamentoCommand(mensagemId, mensagem), cancellationToken);

        return true;
    }
}
