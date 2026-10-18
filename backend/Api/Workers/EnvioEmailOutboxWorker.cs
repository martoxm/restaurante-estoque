using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Api.Workers;

public sealed class EnvioEmailOutboxWorker : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(15);
    private const int QuantidadePorRodada = 50;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EnvioEmailOutboxWorker> _logger;

    public EnvioEmailOutboxWorker(IServiceScopeFactory scopeFactory, ILogger<EnvioEmailOutboxWorker> logger)
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
                _logger.LogError(ex, "Falha ao processar o outbox de e-mails.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }

    private async Task ProcessarPendentesAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var outbox = scope.ServiceProvider.GetRequiredService<IOutboxEmailRepository>();
        var identity = scope.ServiceProvider.GetRequiredService<IIdentityService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var relogio = scope.ServiceProvider.GetRequiredService<IRelogio>();

        var pendentes = await outbox.ListarPendentesAsync(QuantidadePorRodada, relogio.AgoraUtc, ct);

        foreach (var email in pendentes)
        {
            var destinatarios = await identity.ListarEmailsAdminsAsync(email.RestauranteId, ct);

            if (destinatarios.Count == 0)
            {
                _logger.LogWarning("Restaurante {RestauranteId} não tem Admin com e-mail: aviso '{Assunto}' descartado.", email.RestauranteId, email.Assunto);
                email.MarcarComoProcessado(relogio.AgoraUtc);
            }
            else if (await emailService.EnviarAsync(destinatarios, email.Assunto, email.CorpoHtml, ct))
            {
                email.MarcarComoProcessado(relogio.AgoraUtc);
            }
            else
            {
                email.RegistrarFalha("Falha ao enviar o e-mail (detalhes no log da API).", relogio.AgoraUtc);
            }

            outbox.Atualizar(email);
            await context.SaveChangesAsync(ct);
        }
    }
}
