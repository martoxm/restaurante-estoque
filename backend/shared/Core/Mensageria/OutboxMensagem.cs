using System.Text.Json;
using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Core.Mensageria;

public class OutboxMensagem : Entity
{
    public const int FilaMaxLength = 100;
    public const int TipoMaxLength = 200;
    public const int UltimoErroMaxLength = 2000;

    public string Fila { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = string.Empty;
    public string Conteudo { get; private set; } = string.Empty;
    public int Tentativas { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime ProximaTentativaEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }
    public string? UltimoErro { get; private set; }

    private OutboxMensagem()
    {
    }

    public static OutboxMensagem Criar<TMensagem>(string fila, TMensagem mensagem, DateTime criadoEm)
        where TMensagem : class
    {
        if (string.IsNullOrWhiteSpace(fila))
            throw new DomainException("A fila da mensagem é obrigatória.");

        if (mensagem is null)
            throw new DomainException("A mensagem é obrigatória.");

        return new OutboxMensagem
        {
            Fila = fila.Trim(),
            Tipo = typeof(TMensagem).Name,
            Conteudo = JsonSerializer.Serialize(mensagem),
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

        var mensagem = string.IsNullOrWhiteSpace(erro) ? "Falha ao publicar a mensagem." : erro.Trim();
        UltimoErro = mensagem.Length > UltimoErroMaxLength ? mensagem[..UltimoErroMaxLength] : mensagem;

        var minutos = Math.Min(60, Math.Pow(2, Math.Min(Tentativas, 6)));
        ProximaTentativaEm = agora.AddMinutes(minutos);
    }
}
