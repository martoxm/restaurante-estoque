using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RestauranteEstoque.Pagamentos.Infrastructure.Gateways;

public sealed class SimuladorWebhookGatewayFalso : BackgroundService
{
    public const string NomeHttpClient = "gateway-falso-webhook";
    private const int TentativasDeEnvio = 3;

    private readonly ChannelReader<CobrancaFalsa> _fila;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GatewayFalsoSettings _settings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<SimuladorWebhookGatewayFalso> _logger;

    public SimuladorWebhookGatewayFalso(
        Channel<CobrancaFalsa> fila,
        IHttpClientFactory httpClientFactory,
        GatewayFalsoSettings settings,
        TimeProvider timeProvider,
        ILogger<SimuladorWebhookGatewayFalso> logger)
    {
        _fila = fila.Reader;
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var cobranca in _fila.ReadAllAsync(stoppingToken))
        {
            _ = Task.Run(() => AvisarAsync(cobranca, stoppingToken), stoppingToken);
        }
    }

    private async Task AvisarAsync(CobrancaFalsa cobranca, CancellationToken ct)
    {
        try
        {
            if (_settings.AtrasoMs > 0)
                await Task.Delay(_settings.AtrasoMs, ct);

            var motivo = GatewayPagamentoFalso.Decidir(cobranca.Pedido.TokenCartao);
            var evento = new EventoWebhookGatewayFalso(
                Guid.NewGuid(),
                motivo is null ? EventoWebhookGatewayFalso.TipoAprovada : EventoWebhookGatewayFalso.TipoRecusada,
                cobranca.IdTransacao,
                cobranca.Pedido.PagamentoId,
                cobranca.Pedido.RestauranteId,
                motivo,
                _timeProvider.GetUtcNow().UtcDateTime);

            var corpo = JsonSerializer.Serialize(evento);

            for (var tentativa = 1; tentativa <= TentativasDeEnvio; tentativa++)
            {
                using var requisicao = new HttpRequestMessage(HttpMethod.Post, _settings.UrlWebhook)
                {
                    Content = new StringContent(corpo, Encoding.UTF8, "application/json")
                };
                requisicao.Headers.Add(AssinaturaWebhook.NomeCabecalho,
                    AssinaturaWebhook.Gerar(corpo, _settings.SegredoWebhook, _timeProvider.GetUtcNow()));

                try
                {
                    using var resposta = await _httpClientFactory.CreateClient(NomeHttpClient).SendAsync(requisicao, ct);
                    if (resposta.IsSuccessStatusCode)
                        return;

                    _logger.LogWarning("Webhook do gateway falso devolveu {Status} (tentativa {Tentativa}).", (int)resposta.StatusCode, tentativa);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Webhook do gateway falso não respondeu (tentativa {Tentativa}).", tentativa);
                }

                await Task.Delay(TimeSpan.FromSeconds(5 * tentativa), ct);
            }

            _logger.LogError("Gateway falso desistiu de avisar a cobrança {IdTransacao}.", cobranca.IdTransacao);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
        }
    }
}
