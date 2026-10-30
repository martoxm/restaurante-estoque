using System.Text.Json;
using RestauranteEstoque.Core.Domain;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;

namespace RestauranteEstoque.Tests.Domain;

public class OutboxMensagemTests
{
    private static readonly DateTime Agora = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static readonly EnviarEmailMensagem Email = new(Guid.NewGuid(), "Estoque baixo: Arroz", "<p>corpo</p>");

    private static OutboxMensagem CriarMensagemValida() => OutboxMensagem.Criar(Filas.Email, Email, Agora);

    [Fact]
    public void Criar_ComDadosValidos_DeveFicarPendenteParaPublicacaoImediata()
    {
        var mensagem = OutboxMensagem.Criar("  " + Filas.Email + "  ", Email, Agora);

        Assert.Equal(Filas.Email, mensagem.Fila);
        Assert.Equal(nameof(EnviarEmailMensagem), mensagem.Tipo);
        Assert.Equal(0, mensagem.Tentativas);
        Assert.Equal(Agora, mensagem.ProximaTentativaEm);
        Assert.Null(mensagem.ProcessadoEm);
    }

    [Fact]
    public void Criar_DeveGuardarOConteudoEmJsonQueVoltaIgual()
    {
        var mensagem = CriarMensagemValida();

        Assert.Equal(Email, JsonSerializer.Deserialize<EnviarEmailMensagem>(mensagem.Conteudo));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_SemFila_DeveLancarDomainException(string fila)
    {
        Assert.Throws<DomainException>(() => OutboxMensagem.Criar(fila, Email, Agora));
    }

    [Fact]
    public void MarcarComoProcessado_DeveGuardarADataELimparOErro()
    {
        var mensagem = CriarMensagemValida();
        mensagem.RegistrarFalha("RabbitMQ fora do ar", Agora);

        mensagem.MarcarComoProcessado(Agora.AddMinutes(5));

        Assert.Equal(Agora.AddMinutes(5), mensagem.ProcessadoEm);
        Assert.Null(mensagem.UltimoErro);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(5, 32)]
    [InlineData(6, 60)]
    [InlineData(10, 60)]
    public void RegistrarFalha_DeveEsperarCadaVezMaisAteNoMaximo60Minutos(int falhas, int minutosEsperados)
    {
        var mensagem = CriarMensagemValida();

        for (var i = 0; i < falhas; i++)
            mensagem.RegistrarFalha("RabbitMQ fora do ar", Agora);

        Assert.Equal(falhas, mensagem.Tentativas);
        Assert.Equal(Agora.AddMinutes(minutosEsperados), mensagem.ProximaTentativaEm);
        Assert.Equal("RabbitMQ fora do ar", mensagem.UltimoErro);
        Assert.Null(mensagem.ProcessadoEm);
    }
}
