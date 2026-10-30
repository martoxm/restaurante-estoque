using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace RestauranteEstoque.Pagamentos.Infrastructure.Gateways;

public static class AssinaturaWebhook
{
    public const string NomeCabecalho = "X-Assinatura";

    public static readonly TimeSpan Tolerancia = TimeSpan.FromMinutes(5);

    public static string Gerar(string corpo, string segredo, DateTimeOffset agora)
    {
        var timestamp = agora.ToUnixTimeSeconds();
        return $"t={timestamp},v1={CalcularHmac(timestamp, corpo, segredo)}";
    }

    public static bool Verificar(string corpo, string? cabecalho, string segredo, DateTimeOffset agora)
    {
        if (string.IsNullOrWhiteSpace(segredo) || string.IsNullOrWhiteSpace(cabecalho))
            return false;

        long? timestamp = null;
        string? assinatura = null;
        foreach (var parte in cabecalho.Split(','))
        {
            var chaveValor = parte.Split('=', 2);
            if (chaveValor.Length != 2)
                continue;

            if (chaveValor[0].Trim() == "t" && long.TryParse(chaveValor[1], NumberStyles.None, CultureInfo.InvariantCulture, out var t))
                timestamp = t;
            else if (chaveValor[0].Trim() == "v1")
                assinatura = chaveValor[1].Trim();
        }

        if (timestamp is null || assinatura is null)
            return false;

        var enviadoEm = DateTimeOffset.FromUnixTimeSeconds(timestamp.Value);
        if ((agora - enviadoEm).Duration() > Tolerancia)
            return false;

        var esperado = Encoding.ASCII.GetBytes(CalcularHmac(timestamp.Value, corpo, segredo));
        var recebido = Encoding.ASCII.GetBytes(assinatura.ToLowerInvariant());

        return CryptographicOperations.FixedTimeEquals(esperado, recebido);
    }

    private static string CalcularHmac(long timestamp, string corpo, string segredo)
    {
        var bytes = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(segredo),
            Encoding.UTF8.GetBytes($"{timestamp}.{corpo}"));
        return Convert.ToHexStringLower(bytes);
    }
}
