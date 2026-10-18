using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Tests.Domain;

public class OutboxEmailTests
{
    private static readonly DateTime Agora = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static OutboxEmail CriarEmailValido() =>
        OutboxEmail.Criar(Guid.NewGuid(), "Estoque baixo: Arroz", "<p>corpo</p>", Agora);

    [Fact]
    public void Criar_ComDadosValidos_DeveFicarPendenteParaEnvioImediato()
    {
        var restauranteId = Guid.NewGuid();

        var email = OutboxEmail.Criar(restauranteId, "  Estoque baixo: Arroz  ", "<p>corpo</p>", Agora);

        Assert.Equal(restauranteId, email.RestauranteId);
        Assert.Equal("Estoque baixo: Arroz", email.Assunto);
        Assert.Equal(0, email.Tentativas);
        Assert.Equal(Agora, email.ProximaTentativaEm);
        Assert.Null(email.ProcessadoEm);
    }

    [Fact]
    public void Criar_SemRestaurante_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => OutboxEmail.Criar(Guid.Empty, "Assunto", "<p>corpo</p>", Agora));
    }

    [Theory]
    [InlineData("", "<p>corpo</p>")]
    [InlineData("Assunto", "  ")]
    public void Criar_SemAssuntoOuCorpo_DeveLancarDomainException(string assunto, string corpo)
    {
        Assert.Throws<DomainException>(() => OutboxEmail.Criar(Guid.NewGuid(), assunto, corpo, Agora));
    }

    [Fact]
    public void Criar_ComAssuntoMuitoLongo_DeveCortarNoLimite()
    {
        var email = OutboxEmail.Criar(Guid.NewGuid(), new string('a', 300), "<p>corpo</p>", Agora);

        Assert.Equal(OutboxEmail.AssuntoMaxLength, email.Assunto.Length);
    }

    [Fact]
    public void MarcarComoProcessado_DeveGuardarADataELimparOErro()
    {
        var email = CriarEmailValido();
        email.RegistrarFalha("SMTP fora do ar", Agora);

        email.MarcarComoProcessado(Agora.AddMinutes(5));

        Assert.Equal(Agora.AddMinutes(5), email.ProcessadoEm);
        Assert.Null(email.UltimoErro);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(5, 32)]
    [InlineData(6, 60)]
    [InlineData(10, 60)]
    public void RegistrarFalha_DeveEsperarCadaVezMaisAteNoMaximo60Minutos(int falhas, int minutosEsperados)
    {
        var email = CriarEmailValido();

        for (var i = 0; i < falhas; i++)
            email.RegistrarFalha("SMTP fora do ar", Agora);

        Assert.Equal(falhas, email.Tentativas);
        Assert.Equal(Agora.AddMinutes(minutosEsperados), email.ProximaTentativaEm);
        Assert.Equal("SMTP fora do ar", email.UltimoErro);
        Assert.Null(email.ProcessadoEm);
    }
}
