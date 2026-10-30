using System.Text;
using RabbitMQ.Client;

namespace RestauranteEstoque.Core.Mensageria;

public sealed class RabbitMqPublicador : IPublicadorMensagens, IAsyncDisposable
{
    private readonly ConexaoRabbitMq _conexao;
    private readonly SemaphoreSlim _trava = new(1, 1);
    private readonly HashSet<string> _filasDeclaradas = new();
    private IChannel? _canal;

    public RabbitMqPublicador(ConexaoRabbitMq conexao)
    {
        _conexao = conexao;
    }

    public async Task PublicarAsync(string fila, string tipo, string conteudoJson, Guid mensagemId, CancellationToken cancellationToken)
    {
        await _trava.WaitAsync(cancellationToken);
        try
        {
            if (_canal is not { IsOpen: true })
            {
                var conexao = await _conexao.ObterAsync(cancellationToken);

                _canal = await conexao.CreateChannelAsync(
                    new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true),
                    cancellationToken);
                _filasDeclaradas.Clear();
            }

            if (_filasDeclaradas.Add(fila))
                await Filas.DeclararAsync(_canal, fila, cancellationToken);

            var propriedades = new BasicProperties
            {
                MessageId = mensagemId.ToString(),
                Type = tipo,
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await _canal.BasicPublishAsync(exchange: string.Empty, routingKey: fila, mandatory: false,
                basicProperties: propriedades, body: Encoding.UTF8.GetBytes(conteudoJson), cancellationToken: cancellationToken);
        }
        finally
        {
            _trava.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_canal is not null)
            await _canal.DisposeAsync();

        _trava.Dispose();
    }
}
