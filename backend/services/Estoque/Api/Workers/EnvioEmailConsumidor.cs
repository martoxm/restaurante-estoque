using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;

namespace RestauranteEstoque.Api.Workers;

public sealed class EnvioEmailConsumidor : ConsumidorFilaBase<EnviarEmailMensagem>
{
    private readonly ILogger<EnvioEmailConsumidor> _logger;

    public EnvioEmailConsumidor(ConexaoRabbitMq conexao, IServiceScopeFactory scopeFactory, ILogger<EnvioEmailConsumidor> logger)
        : base(conexao, scopeFactory, logger)
    {
        _logger = logger;
    }

    protected override string Fila => Filas.Email;

    protected override async Task<bool> ProcessarAsync(EnviarEmailMensagem mensagem, Guid mensagemId, IServiceProvider servicos, CancellationToken cancellationToken)
    {
        var identidade = servicos.GetRequiredService<IIdentidadeClient>();
        var emailService = servicos.GetRequiredService<IEmailService>();

        var destinatarios = await identidade.ListarEmailsAdminsAsync(mensagem.RestauranteId, cancellationToken);

        if (destinatarios.Count == 0)
        {
            _logger.LogWarning("Restaurante {RestauranteId} não tem Admin com e-mail: aviso '{Assunto}' descartado.", mensagem.RestauranteId, mensagem.Assunto);
            return true;
        }

        return await emailService.EnviarAsync(destinatarios, mensagem.Assunto, mensagem.CorpoHtml, cancellationToken);
    }
}
