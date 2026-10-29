using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RestauranteEstoque.Core.Api;
using RestauranteEstoque.Infrastructure.Integracoes.Identidade;

namespace RestauranteEstoque.Tests.Integracoes;

public class IdentidadeClientTests
{
    [Fact]
    public async Task ListarEmailsAdminsAsync_DeveChamarARotaInternaELerOsEmails()
    {
        var restauranteId = Guid.NewGuid();
        var handler = new HandlerFalso(HttpStatusCode.OK, """{"emails":["admin@teste.com"]}""");
        var client = CriarClient(handler);

        var emails = await client.ListarEmailsAdminsAsync(restauranteId, CancellationToken.None);

        Assert.Equal(["admin@teste.com"], emails);
        Assert.Equal($"https://identidade.teste/api/v1/interno/restaurantes/{restauranteId}/emails-admins",
            handler.UltimaRequisicao!.RequestUri!.ToString());
        Assert.True(handler.UltimaRequisicao.Headers.Contains(CorrelationIdExtensions.NomeCabecalho));
    }

    [Fact]
    public async Task ListarEmailsAdminsAsync_QuandoAIdentidadeRecusa_DeveLancarExcecao()
    {
        var client = CriarClient(new HandlerFalso(HttpStatusCode.Unauthorized, ""));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => client.ListarEmailsAdminsAsync(Guid.NewGuid(), CancellationToken.None));
    }

    private static IdentidadeClient CriarClient(HandlerFalso handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://identidade.teste/") };
        return new IdentidadeClient(httpClient, new HttpContextAccessor(), NullLogger<IdentidadeClient>.Instance);
    }

    private sealed class HandlerFalso(HttpStatusCode status, string corpo) : HttpMessageHandler
    {
        public HttpRequestMessage? UltimaRequisicao { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            UltimaRequisicao = request;
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(corpo, Encoding.UTF8, "application/json")
            });
        }
    }
}
