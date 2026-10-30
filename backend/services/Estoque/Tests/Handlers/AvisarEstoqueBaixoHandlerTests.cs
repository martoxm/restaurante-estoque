using System.Text.Json;
using RestauranteEstoque.Core.Application.Events;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Application.UseCases.Produtos;
using RestauranteEstoque.Domain.Events;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class AvisarEstoqueBaixoHandlerTests
{
    private static (AvisarEstoqueBaixoHandler Handler, FakeOutboxMensagemRepository Outbox, FakeRelogio Relogio) CriarHandler()
    {
        var outbox = new FakeOutboxMensagemRepository();
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

        var linha = Assert.Single(outbox.Mensagens);
        Assert.Equal(Filas.Email, linha.Fila);
        Assert.Equal(nameof(EnviarEmailMensagem), linha.Tipo);
        Assert.Equal(relogio.AgoraUtc, linha.CriadoEm);

        var email = JsonSerializer.Deserialize<EnviarEmailMensagem>(linha.Conteudo)!;
        Assert.Equal(restauranteId, email.RestauranteId);
        Assert.Equal("Estoque baixo: Arroz", email.Assunto);
        Assert.Contains("<b>Arroz</b>", email.CorpoHtml);
        Assert.Contains("<b>7</b>", email.CorpoHtml);
    }

    [Fact]
    public async Task Handle_NomeComHtml_DeveEscaparParaNaoInjetarNoEmail()
    {
        var (handler, outbox, _) = CriarHandler();

        await handler.Handle(Notificacao("<a href=\"http://golpe\">clique</a>", Guid.NewGuid()), CancellationToken.None);

        var corpo = JsonSerializer.Deserialize<EnviarEmailMensagem>(Assert.Single(outbox.Mensagens).Conteudo)!.CorpoHtml;
        Assert.DoesNotContain("<a href", corpo);
        Assert.Contains("&lt;a href=", corpo);
    }
}
