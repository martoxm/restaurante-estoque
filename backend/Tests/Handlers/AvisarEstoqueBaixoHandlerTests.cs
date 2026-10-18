using RestauranteEstoque.Application.Common.Events;
using RestauranteEstoque.Application.UseCases.Produtos;
using RestauranteEstoque.Domain.Events;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class AvisarEstoqueBaixoHandlerTests
{
    private static (AvisarEstoqueBaixoHandler Handler, FakeOutboxEmailRepository Outbox, FakeRelogio Relogio) CriarHandler()
    {
        var outbox = new FakeOutboxEmailRepository();
        var relogio = new FakeRelogio();
        var handler = new AvisarEstoqueBaixoHandler(outbox, relogio);
        return (handler, outbox, relogio);
    }

    private static DomainEventNotification<EstoqueFicouBaixoEvent> Notificacao(string nomeProduto, Guid restauranteId) =>
        new(new EstoqueFicouBaixoEvent(Guid.NewGuid(), nomeProduto, 7, restauranteId));

    [Fact]
    public async Task Handle_ProdutoComRestaurante_DeveGravarEmailNoOutbox()
    {
        var (handler, outbox, relogio) = CriarHandler();
        var restauranteId = Guid.NewGuid();

        await handler.Handle(Notificacao("Arroz", restauranteId), CancellationToken.None);

        var email = Assert.Single(outbox.Emails);
        Assert.Equal(restauranteId, email.RestauranteId);
        Assert.Equal("Estoque baixo: Arroz", email.Assunto);
        Assert.Contains("<b>Arroz</b>", email.CorpoHtml);
        Assert.Contains("<b>7</b>", email.CorpoHtml);
        Assert.Equal(relogio.AgoraUtc, email.CriadoEm);
    }

    [Fact]
    public async Task Handle_NomeComHtml_DeveEscaparParaNaoInjetarNoEmail()
    {
        var (handler, outbox, _) = CriarHandler();

        await handler.Handle(Notificacao("<a href=\"http://golpe\">clique</a>", Guid.NewGuid()), CancellationToken.None);

        var corpo = Assert.Single(outbox.Emails).CorpoHtml;
        Assert.DoesNotContain("<a href", corpo);
        Assert.Contains("&lt;a href=", corpo);
    }
}
