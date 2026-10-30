using MediatR;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Repositories;
using RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;

namespace RestauranteEstoque.Pagamentos.Api.Workers;

public sealed class ProcessamentoPagamentosWorker : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(5);
    private const int QuantidadePorRodada = 20;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProcessamentoPagamentosWorker> _logger;

    public ProcessamentoPagamentosWorker(IServiceScopeFactory scopeFactory, ILogger<ProcessamentoPagamentosWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarPendentesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao processar os pagamentos pendentes.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    private async Task ProcessarPendentesAsync(CancellationToken ct)
    {
        IReadOnlyList<PagamentoPendente> pendentes;
        using (var scope = _scopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IPagamentoRepository>();
            pendentes = await repository.ListarPendentesDeTodosAsync(QuantidadePorRodada, ct);
        }

        foreach (var pendente in pendentes)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                scope.ServiceProvider.GetRequiredService<IDefinidorRestauranteAtual>().Definir(pendente.RestauranteId);
                await scope.ServiceProvider.GetRequiredService<ISender>()
                    .Send(new ProcessarPagamentoCommand(pendente.PagamentoId), ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _logger.LogError(ex, "Falha ao processar o pagamento {PagamentoId}.", pendente.PagamentoId);
            }
        }
    }
}
