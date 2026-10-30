using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RestauranteEstoque.Core.Mensageria;

public abstract class ConsumidorFilaBase<TMensagem> : BackgroundService
{
    private static readonly TimeSpan EsperaParaReconectar = TimeSpan.FromSeconds(15);

    public const string CabecalhoTentativas = "x-tentativas";
    public const int MaximoTentativas = 5;

    private readonly ConexaoRabbitMq _conexao;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    protected ConsumidorFilaBase(ConexaoRabbitMq conexao, IServiceScopeFactory scopeFactory, ILogger logger)
    {
        _conexao = conexao;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected abstract string Fila { get; }

    protected abstract Task<bool> ProcessarAsync(TMensagem mensagem, Guid mensagemId, IServiceProvider servicos, CancellationToken cancellationToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ConsumirAsync(stoppingToken);
                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Não deu pra conectar na fila {Fila}. Tentando de novo em {Segundos} s.", Fila, EsperaParaReconectar.TotalSeconds);
                await Task.Delay(EsperaParaReconectar, stoppingToken);
            }
        }
    }

    private async Task ConsumirAsync(CancellationToken stoppingToken)
    {
        var conexao = await _conexao.ObterAsync(stoppingToken);
        await using var canal = await conexao.CreateChannelAsync(cancellationToken: stoppingToken);

        await Filas.DeclararAsync(canal, Fila, stoppingToken);

        await canal.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);

        var consumidor = new AsyncEventingBasicConsumer(canal);
        consumidor.ReceivedAsync += async (_, entrega) =>
        {
            var tratada = false;
            try
            {
                var mensagem = JsonSerializer.Deserialize<TMensagem>(entrega.Body.Span)
                    ?? throw new InvalidOperationException("A mensagem chegou vazia.");

                Guid.TryParse(entrega.BasicProperties.MessageId, out var mensagemId);

                using var scope = _scopeFactory.CreateScope();
                tratada = await ProcessarAsync(mensagem, mensagemId, scope.ServiceProvider, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a mensagem {MensagemId} da fila {Fila}.", entrega.BasicProperties.MessageId, Fila);
            }

            if (tratada)
            {
                await canal.BasicAckAsync(entrega.DeliveryTag, multiple: false);
                return;
            }

            var tentativas = LerTentativas(entrega.BasicProperties) + 1;
            if (tentativas >= MaximoTentativas)
            {
                _logger.LogError("Mensagem {MensagemId} falhou {Tentativas} vezes na fila {Fila}: indo pra dlq.", entrega.BasicProperties.MessageId, tentativas, Fila);
                await canal.BasicNackAsync(entrega.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            var propriedades = new BasicProperties(entrega.BasicProperties)
            {
                Headers = new Dictionary<string, object?>(entrega.BasicProperties.Headers ?? new Dictionary<string, object?>())
                {
                    [CabecalhoTentativas] = tentativas
                }
            };
            await canal.BasicPublishAsync(string.Empty, Fila, mandatory: false, basicProperties: propriedades, body: entrega.Body);
            await canal.BasicAckAsync(entrega.DeliveryTag, multiple: false);
        };

        await canal.BasicConsumeAsync(Fila, autoAck: false, consumer: consumidor, cancellationToken: stoppingToken);
        _logger.LogInformation("Escutando a fila {Fila}.", Fila);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private static int LerTentativas(IReadOnlyBasicProperties propriedades)
    {
        if (propriedades.Headers is null || !propriedades.Headers.TryGetValue(CabecalhoTentativas, out var valor))
            return 0;

        return valor switch
        {
            int numero => numero,
            long numero => (int)numero,
            byte[] texto when int.TryParse(System.Text.Encoding.UTF8.GetString(texto), out var numero) => numero,
            _ => 0
        };
    }
}
