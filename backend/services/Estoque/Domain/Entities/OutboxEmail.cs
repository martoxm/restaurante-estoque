using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Domain.Entities;

public class OutboxEmail : Entity
{
    public const int AssuntoMaxLength = 200;
    public const int UltimoErroMaxLength = 2000;

    public Guid RestauranteId { get; private set; }
    public string Assunto { get; private set; } = string.Empty;
    public string CorpoHtml { get; private set; } = string.Empty;
    public int Tentativas { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime ProximaTentativaEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }
    public string? UltimoErro { get; private set; }

    private OutboxEmail()
    {
    }

    public static OutboxEmail Criar(Guid restauranteId, string assunto, string corpoHtml, DateTime criadoEm)
    {
        if (restauranteId == Guid.Empty)
            throw new DomainException("O e-mail precisa estar vinculado a um restaurante.");

        if (string.IsNullOrWhiteSpace(assunto))
            throw new DomainException("O assunto do e-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(corpoHtml))
            throw new DomainException("O corpo do e-mail é obrigatório.");

        var assuntoLimpo = assunto.Trim();

        return new OutboxEmail
        {
            RestauranteId = restauranteId,
            Assunto = assuntoLimpo.Length > AssuntoMaxLength ? assuntoLimpo[..AssuntoMaxLength] : assuntoLimpo,
            CorpoHtml = corpoHtml,
            CriadoEm = criadoEm,
            ProximaTentativaEm = criadoEm
        };
    }

    public void MarcarComoProcessado(DateTime agora)
    {
        ProcessadoEm = agora;
        UltimoErro = null;
    }

    public void RegistrarFalha(string erro, DateTime agora)
    {
        Tentativas++;

        var mensagem = string.IsNullOrWhiteSpace(erro) ? "Falha ao enviar o e-mail." : erro.Trim();
        UltimoErro = mensagem.Length > UltimoErroMaxLength ? mensagem[..UltimoErroMaxLength] : mensagem;

        var minutos = Math.Min(60, Math.Pow(2, Math.Min(Tentativas, 6)));
        ProximaTentativaEm = agora.AddMinutes(minutos);
    }
}
