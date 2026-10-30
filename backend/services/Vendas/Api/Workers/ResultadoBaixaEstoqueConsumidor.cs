using MediatR;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Vendas.Application.UseCases.Vendas;

namespace RestauranteEstoque.Vendas.Api.Workers;

public sealed class ResultadoBaixaEstoqueConsumidor : ConsumidorFilaBase<ResultadoBaixaEstoqueMensagem>
{
    private readonly ILogger<ResultadoBaixaEstoqueConsumidor> _logger;

    public ResultadoBaixaEstoqueConsumidor(ConexaoRabbitMq conexao, IServiceScopeFactory scopeFactory, ILogger<ResultadoBaixaEstoqueConsumidor> logger)
        : base(conexao, scopeFactory, logger)
    {
        _logger = logger;
    }

    protected override string Fila => Filas.ResultadoBaixaEstoque;

    protected override async Task<bool> ProcessarAsync(ResultadoBaixaEstoqueMensagem mensagem, Guid mensagemId, IServiceProvider servicos, CancellationToken cancellationToken)
    {
        if (mensagemId == Guid.Empty)
        {
            _logger.LogError("Resultado da baixa da venda {VendaId} chegou sem MessageId.", mensagem.VendaId);
            return false;
        }

        var sender = servicos.GetRequiredService<ISender>();
        await sender.Send(new ProcessarResultadoBaixaCommand(mensagemId, mensagem), cancellationToken);

        return true;
    }
}
