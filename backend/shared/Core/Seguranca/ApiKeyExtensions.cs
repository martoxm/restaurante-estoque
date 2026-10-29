using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RestauranteEstoque.Core.Seguranca;

public static class ApiKeyExtensions
{
    public const string NomeCabecalho = "x-api-key";
    public const string ChaveConfiguracao = "ApiInterna:Chave";

    public static TBuilder RequireApiKey<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var chaveEsperada = configuration[ChaveConfiguracao];
            var chaveRecebida = context.HttpContext.Request.Headers[NomeCabecalho].ToString();

            if (string.IsNullOrWhiteSpace(chaveEsperada) || !ChavesIguais(chaveEsperada, chaveRecebida))
                return Results.Unauthorized();

            return await next(context);
        });

        return builder;
    }

    private static bool ChavesIguais(string esperada, string recebida)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(esperada),
            Encoding.UTF8.GetBytes(recebida));
    }
}
