using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Core.Api;

namespace RestauranteEstoque.Infrastructure.Integracoes.Identidade;

public class IdentidadeClient : IIdentidadeClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IdentidadeClient> _logger;

    public IdentidadeClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<IdentidadeClient> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IReadOnlyList<string>> ListarEmailsAdminsAsync(Guid restauranteId, CancellationToken cancellationToken)
    {
        using var requisicao = new HttpRequestMessage(HttpMethod.Get, $"api/v1/interno/restaurantes/{restauranteId}/emails-admins");

        var correlationId = _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
        requisicao.Headers.Add(CorrelationIdExtensions.NomeCabecalho, correlationId);

        using var resposta = await _httpClient.SendAsync(requisicao, cancellationToken);

        if (!resposta.IsSuccessStatusCode)
        {
            _logger.LogError(
                "A Identidade respondeu {StatusCode} ao listar os Admins do restaurante {RestauranteId} (CorrelationId {CorrelationId}).",
                (int)resposta.StatusCode, restauranteId, correlationId);
            resposta.EnsureSuccessStatusCode();
        }

        var corpo = await resposta.Content.ReadFromJsonAsync<EmailsAdminsResposta>(cancellationToken);
        return corpo?.Emails ?? [];
    }

    private sealed record EmailsAdminsResposta(IReadOnlyList<string> Emails);
}
