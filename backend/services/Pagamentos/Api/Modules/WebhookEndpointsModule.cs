using System.Text.Json;
using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Core.Api;
using RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;
using RestauranteEstoque.Pagamentos.Infrastructure.Gateways;

namespace RestauranteEstoque.Pagamentos.Api.Modules;

public class WebhookEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        app.MapPost("/api/v{version:apiVersion}/webhooks/gateway-falso", ReceberWebhook)
            .WithApiVersionSet(versionSet)
            .WithTags("Webhooks")
            .AllowAnonymous()
            .WithName("ReceberWebhookGatewayFalso")
            .WithSummary("Recebe o aviso do gateway de pagamento")
            .WithDescription("Chamado pelo gateway (falso), não pelo front. Exige o cabeçalho X-Assinatura (t=...,v1=HMAC-SHA256 de \"{t}.{corpo}\" com o segredo do webhook). Assinatura inválida ou com mais de 5 minutos: 401.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        app.MapPost("/api/v{version:apiVersion}/webhooks/stripe", ReceberWebhookStripe)
            .WithApiVersionSet(versionSet)
            .WithTags("Webhooks")
            .AllowAnonymous()
            .WithName("ReceberWebhookStripe")
            .WithSummary("Recebe o aviso do Stripe")
            .WithDescription("Chamado pelo Stripe (ou pelo stripe listen). Exige o cabeçalho Stripe-Signature, conferido com o segredo whsec_ do webhook. Trata payment_intent.succeeded e payment_intent.payment_failed; os outros eventos só recebem 200.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> ReceberWebhookStripe(
        HttpRequest request,
        StripeSettings settings,
        ISender sender,
        ILogger<WebhookEndpointsModule> logger,
        CancellationToken cancellationToken)
    {
        using var leitor = new StreamReader(request.Body);
        var corpo = await leitor.ReadToEndAsync(cancellationToken);

        var resultado = WebhookStripe.Interpretar(corpo, request.Headers[WebhookStripe.NomeCabecalho].ToString(), settings.SegredoWebhook);
        if (!resultado.AssinaturaValida)
        {
            logger.LogWarning("Webhook do Stripe recusado: assinatura inválida ou vencida.");
            return Results.Unauthorized();
        }

        if (resultado.Comando is not null)
            await sender.Send(resultado.Comando, cancellationToken);

        return Results.Ok();
    }

    private static async Task<IResult> ReceberWebhook(
        HttpRequest request,
        GatewayFalsoSettings settings,
        TimeProvider timeProvider,
        ISender sender,
        ILogger<WebhookEndpointsModule> logger,
        CancellationToken cancellationToken)
    {
        using var leitor = new StreamReader(request.Body);
        var corpo = await leitor.ReadToEndAsync(cancellationToken);

        var assinatura = request.Headers[AssinaturaWebhook.NomeCabecalho].ToString();
        if (!AssinaturaWebhook.Verificar(corpo, assinatura, settings.SegredoWebhook, timeProvider.GetUtcNow()))
        {
            logger.LogWarning("Webhook do gateway recusado: assinatura inválida ou vencida.");
            return Results.Unauthorized();
        }

        EventoWebhookGatewayFalso? evento;
        try
        {
            evento = JsonSerializer.Deserialize<EventoWebhookGatewayFalso>(corpo);
        }
        catch (JsonException)
        {
            return Results.BadRequest();
        }

        if (evento is null)
            return Results.BadRequest();

        if (evento.Tipo is not (EventoWebhookGatewayFalso.TipoAprovada or EventoWebhookGatewayFalso.TipoRecusada))
            return Results.Ok();

        await sender.Send(new ProcessarWebhookGatewayCommand(
            evento.PagamentoId,
            evento.RestauranteId,
            evento.IdTransacao,
            evento.Tipo == EventoWebhookGatewayFalso.TipoAprovada,
            evento.Motivo), cancellationToken);

        return Results.Ok();
    }
}
