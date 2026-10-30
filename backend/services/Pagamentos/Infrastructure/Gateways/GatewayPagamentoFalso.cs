using System.Threading.Channels;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Services;

namespace RestauranteEstoque.Pagamentos.Infrastructure.Gateways;

public class GatewayPagamentoFalso : IGatewayPagamento
{
    public const string TokenAprovado = "tok_aprovado";
    public const string TokenSaldoInsuficiente = "tok_saldo_insuficiente";
    public const string TokenFraude = "tok_fraude";
    public const string TokenGatewayFora = "tok_gateway_fora";

    private readonly ChannelWriter<CobrancaFalsa> _fila;

    public GatewayPagamentoFalso(Channel<CobrancaFalsa> fila)
    {
        _fila = fila.Writer;
    }

    public async Task<CobrancaAceita> CobrarAsync(PedidoCobranca pedido, CancellationToken cancellationToken)
    {
        if (pedido.TokenCartao == TokenGatewayFora)
            throw new GatewayIndisponivelException("Gateway falso fora do ar (token de teste tok_gateway_fora).");

        var idTransacao = $"fake_tr_{pedido.PagamentoId:N}";
        await _fila.WriteAsync(new CobrancaFalsa(pedido, idTransacao), cancellationToken);

        return new CobrancaAceita(idTransacao);
    }

    public static string? Decidir(string tokenCartao) => tokenCartao switch
    {
        TokenAprovado => null,
        TokenSaldoInsuficiente => "Saldo insuficiente.",
        TokenFraude => "Recusado por suspeita de fraude.",
        _ => "Cartão inválido."
    };
}

public record CobrancaFalsa(PedidoCobranca Pedido, string IdTransacao);

public record EventoWebhookGatewayFalso(
    Guid IdEvento,
    string Tipo,
    string IdTransacao,
    Guid PagamentoId,
    Guid RestauranteId,
    string? Motivo,
    DateTime CriadoEm)
{
    public const string TipoAprovada = "cobranca.aprovada";
    public const string TipoRecusada = "cobranca.recusada";
}

public class GatewayFalsoSettings
{
    public int AtrasoMs { get; set; } = 2000;

    public string UrlWebhook { get; set; } = "";

    public string SegredoWebhook { get; set; } = "";
}
