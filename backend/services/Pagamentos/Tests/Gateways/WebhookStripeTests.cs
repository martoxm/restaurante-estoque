using RestauranteEstoque.Pagamentos.Infrastructure.Gateways;

namespace RestauranteEstoque.Pagamentos.Tests.Gateways;

public class WebhookStripeTests
{
    private const string Segredo = "whsec_teste";
    private static readonly Guid PagamentoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid RestauranteId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static string Evento(string tipo, bool comMetadados = true, string? erro = null) => $$"""
        {
          "id": "evt_teste",
          "object": "event",
          "api_version": "2025-01-27.acacia",
          "created": 1730300000,
          "type": "{{tipo}}",
          "data": {
            "object": {
              "id": "pi_teste",
              "object": "payment_intent",
              "amount": 1500,
              "currency": "brl",
              "status": "succeeded",
              "metadata": {{(comMetadados ? $$"""{ "pagamentoId": "{{PagamentoId}}", "restauranteId": "{{RestauranteId}}" }""" : "{}")}},
              "last_payment_error": {{(erro is null ? "null" : $$"""{ "message": "{{erro}}", "type": "card_error" }""")}}
            }
          }
        }
        """;

    private static string Assinar(string corpo) => AssinaturaWebhook.Gerar(corpo, Segredo, DateTimeOffset.UtcNow);

    [Fact]
    public void Interpretar_PagamentoAprovado_ViraComandoAprovado()
    {
        var corpo = Evento("payment_intent.succeeded");

        var resultado = WebhookStripe.Interpretar(corpo, Assinar(corpo), Segredo);

        Assert.True(resultado.AssinaturaValida);
        Assert.NotNull(resultado.Comando);
        Assert.True(resultado.Comando!.Aprovado);
        Assert.Equal(PagamentoId, resultado.Comando.PagamentoId);
        Assert.Equal(RestauranteId, resultado.Comando.RestauranteId);
        Assert.Equal("pi_teste", resultado.Comando.IdTransacao);
    }

    [Fact]
    public void Interpretar_PagamentoRecusado_LevaOMotivo()
    {
        var corpo = Evento("payment_intent.payment_failed", erro: "Your card was declined.");

        var resultado = WebhookStripe.Interpretar(corpo, Assinar(corpo), Segredo);

        Assert.False(resultado.Comando!.Aprovado);
        Assert.Equal("Your card was declined.", resultado.Comando.Motivo);
    }

    [Fact]
    public void Interpretar_CorpoAlterado_RecusaAAssinatura()
    {
        var corpo = Evento("payment_intent.payment_failed");
        var assinatura = Assinar(corpo);

        var resultado = WebhookStripe.Interpretar(corpo.Replace("payment_failed", "succeeded"), assinatura, Segredo);

        Assert.False(resultado.AssinaturaValida);
    }

    [Fact]
    public void Interpretar_SegredoErrado_RecusaAAssinatura()
    {
        var corpo = Evento("payment_intent.succeeded");

        var resultado = WebhookStripe.Interpretar(corpo, AssinaturaWebhook.Gerar(corpo, "whsec_outro", DateTimeOffset.UtcNow), Segredo);

        Assert.False(resultado.AssinaturaValida);
    }

    [Fact]
    public void Interpretar_EventoQueNaoInteressa_SoConfirma()
    {
        var corpo = Evento("payment_intent.created");

        var resultado = WebhookStripe.Interpretar(corpo, Assinar(corpo), Segredo);

        Assert.True(resultado.AssinaturaValida);
        Assert.Null(resultado.Comando);
    }

    [Fact]
    public void Interpretar_PaymentIntentSemNossosMetadados_Ignora()
    {
        var corpo = Evento("payment_intent.succeeded", comMetadados: false);

        var resultado = WebhookStripe.Interpretar(corpo, Assinar(corpo), Segredo);

        Assert.True(resultado.AssinaturaValida);
        Assert.Null(resultado.Comando);
    }
}
