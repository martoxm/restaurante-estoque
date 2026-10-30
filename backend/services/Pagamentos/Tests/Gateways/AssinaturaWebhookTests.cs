using RestauranteEstoque.Pagamentos.Infrastructure.Gateways;

namespace RestauranteEstoque.Pagamentos.Tests.Gateways;

public class AssinaturaWebhookTests
{
    private const string Segredo = "whsec_teste";
    private const string Corpo = "{\"Tipo\":\"cobranca.recusada\"}";
    private static readonly DateTimeOffset Agora = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Verificar_AssinaturaCerta_Aceita()
    {
        var cabecalho = AssinaturaWebhook.Gerar(Corpo, Segredo, Agora);

        Assert.True(AssinaturaWebhook.Verificar(Corpo, cabecalho, Segredo, Agora));
    }

    [Fact]
    public void Verificar_CorpoAlterado_Recusa()
    {
        var cabecalho = AssinaturaWebhook.Gerar(Corpo, Segredo, Agora);

        Assert.False(AssinaturaWebhook.Verificar(Corpo.Replace("recusada", "aprovada"), cabecalho, Segredo, Agora));
    }

    [Fact]
    public void Verificar_SegredoErrado_Recusa()
    {
        var cabecalho = AssinaturaWebhook.Gerar(Corpo, "segredo-chutado", Agora);

        Assert.False(AssinaturaWebhook.Verificar(Corpo, cabecalho, Segredo, Agora));
    }

    [Fact]
    public void Verificar_AssinaturaVelha_Recusa()
    {
        var cabecalho = AssinaturaWebhook.Gerar(Corpo, Segredo, Agora);

        Assert.False(AssinaturaWebhook.Verificar(Corpo, cabecalho, Segredo, Agora.AddMinutes(10)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("v1=abc")]
    [InlineData("t=abc,v1=abc")]
    public void Verificar_CabecalhoFaltandoOuQuebrado_Recusa(string? cabecalho)
    {
        Assert.False(AssinaturaWebhook.Verificar(Corpo, cabecalho, Segredo, Agora));
    }

    [Fact]
    public void Verificar_SemSegredoConfigurado_Recusa()
    {
        var cabecalho = AssinaturaWebhook.Gerar(Corpo, "", Agora);

        Assert.False(AssinaturaWebhook.Verificar(Corpo, cabecalho, "", Agora));
    }
}
