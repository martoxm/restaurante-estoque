using RestauranteEstoque.Identidade.Application.Abstractions.Repositories;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;

namespace RestauranteEstoque.Identidade.Api.Workers;

public class LimpezaRefreshTokensWorker : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(12);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LimpezaRefreshTokensWorker> _logger;

    public LimpezaRefreshTokensWorker(IServiceScopeFactory scopeFactory, ILogger<LimpezaRefreshTokensWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Intervalo);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
                var relogio = scope.ServiceProvider.GetRequiredService<IRelogio>();

                var removidos = await repository.RemoverVencidosAsync(relogio.AgoraUtc, stoppingToken);
                _logger.LogInformation("Limpeza de refresh tokens: {Removidos} removidos.", removidos);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Falha na limpeza de refresh tokens.");
            }
        }
    }
}
