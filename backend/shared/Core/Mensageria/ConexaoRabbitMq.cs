using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RestauranteEstoque.Core.Mensageria;

public sealed class ConexaoRabbitMq : IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly SemaphoreSlim _trava = new(1, 1);
    private IConnection? _conexao;

    public ConexaoRabbitMq(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<IConnection> ObterAsync(CancellationToken cancellationToken)
    {
        if (_conexao is { IsOpen: true })
            return _conexao;

        await _trava.WaitAsync(cancellationToken);
        try
        {
            if (_conexao is { IsOpen: true })
                return _conexao;

            if (_conexao is not null)
                await _conexao.DisposeAsync();

            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Porta,
                UserName = _settings.Usuario,
                Password = _settings.Senha,
                AutomaticRecoveryEnabled = true
            };

            _conexao = await factory.CreateConnectionAsync(cancellationToken);
            return _conexao;
        }
        finally
        {
            _trava.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_conexao is not null)
            await _conexao.DisposeAsync();

        _trava.Dispose();
    }
}
