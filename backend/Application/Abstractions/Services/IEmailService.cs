namespace RestauranteEstoque.Application.Abstractions.Services;

public interface IEmailService
{
    Task<bool> EnviarAsync(IEnumerable<string> destinatarios, string assunto, string corpoHtml, CancellationToken ct = default);
}
