using Microsoft.Extensions.Logging;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Services;
using Stripe;

namespace RestauranteEstoque.Pagamentos.Infrastructure.Gateways;

public class GatewayStripe : IGatewayPagamento
{
    private readonly PaymentIntentService _paymentIntents;
    private readonly ILogger<GatewayStripe> _logger;

    public GatewayStripe(StripeClient client, ILogger<GatewayStripe> logger)
    {
        _paymentIntents = new PaymentIntentService(client);
        _logger = logger;
    }

    public async Task<CobrancaAceita> CobrarAsync(PedidoCobranca pedido, CancellationToken cancellationToken)
    {
        var opcoes = new PaymentIntentCreateOptions
        {
            Amount = (long)Math.Round(pedido.Valor * 100m, MidpointRounding.AwayFromZero),
            Currency = "brl",
            PaymentMethod = pedido.TokenCartao,
            Confirm = true,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            },
            Metadata = new Dictionary<string, string>
            {
                [MetadadosStripe.PagamentoId] = pedido.PagamentoId.ToString(),
                [MetadadosStripe.RestauranteId] = pedido.RestauranteId.ToString()
            }
        };

        var requisicao = new RequestOptions { IdempotencyKey = $"pagamento-{pedido.PagamentoId:N}" };

        try
        {
            var intent = await _paymentIntents.CreateAsync(opcoes, requisicao, cancellationToken);
            return new CobrancaAceita(intent.Id);
        }
        catch (StripeException ex) when (ex.StripeError?.Type == "card_error" && ex.StripeError.PaymentIntent is not null)
        {
            _logger.LogInformation("Stripe recusou o cartão do pagamento {PagamentoId}; a confirmação vem pelo webhook.", pedido.PagamentoId);
            return new CobrancaAceita(ex.StripeError.PaymentIntent.Id);
        }
        catch (StripeException ex) when ((int)ex.HttpStatusCode >= 500 || (int)ex.HttpStatusCode == 429 || ex.StripeError?.Type == "api_error")
        {
            throw new GatewayIndisponivelException($"Stripe indisponível: {ex.Message}");
        }
        catch (StripeException ex)
        {
            throw new CobrancaRecusadaException(ex.StripeError?.Message ?? ex.Message);
        }
        catch (HttpRequestException ex)
        {
            throw new GatewayIndisponivelException($"Sem conexão com o Stripe: {ex.Message}");
        }
    }
}

public static class MetadadosStripe
{
    public const string PagamentoId = "pagamentoId";
    public const string RestauranteId = "restauranteId";
}

public class StripeSettings
{
    public string ChaveSecreta { get; set; } = "";

    public string SegredoWebhook { get; set; } = "";
}
