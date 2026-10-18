using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RestauranteEstoque.Application.Abstractions.Services;

namespace RestauranteEstoque.Infrastructure.EnvioEmail;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> EnviarAsync(IEnumerable<string> destinatarios, string assunto, string corpoHtml, CancellationToken ct = default)
    {
        var lista = destinatarios.Where(d => !string.IsNullOrWhiteSpace(d)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (lista.Count == 0)
            return false;

        if (string.IsNullOrWhiteSpace(_settings.Host) || string.IsNullOrWhiteSpace(_settings.Usuario) || string.IsNullOrWhiteSpace(_settings.Senha))
        {
            _logger.LogWarning("E-mail não enviado: SMTP não configurado (Email:Host/Usuario/Senha). Configure via dotnet user-secrets na Api.");
            return false;
        }

        var mensagem = new MimeMessage();
        var remetente = string.IsNullOrWhiteSpace(_settings.Remetente) ? _settings.Usuario : _settings.Remetente;
        mensagem.From.Add(new MailboxAddress(_settings.NomeRemetente, remetente));
        foreach (var destinatario in lista)
            mensagem.To.Add(MailboxAddress.Parse(destinatario));
        mensagem.Subject = assunto;
        mensagem.Body = new BodyBuilder { HtmlBody = corpoHtml }.ToMessageBody();

        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Porta, SecureSocketOptions.StartTls, ct);
            await smtp.AuthenticateAsync(_settings.Usuario, _settings.Senha, ct);
            await smtp.SendAsync(mensagem, ct);
            await smtp.DisconnectAsync(true, ct);

            _logger.LogInformation("E-mail '{Assunto}' enviado para {Destinatarios}.", assunto, string.Join(", ", lista));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar o e-mail '{Assunto}' para {Destinatarios}.", assunto, string.Join(", ", lista));
            return false;
        }
    }
}
