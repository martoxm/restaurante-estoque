using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Core.Api;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Identidade.Application.UseCases.Restaurantes;
using RestauranteEstoque.Identidade.Contracts.Restaurantes;

namespace RestauranteEstoque.Identidade.Api.Modules;

public class InternoEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/interno")
            .WithApiVersionSet(versionSet)
            .WithTags("Interno")
            .AllowAnonymous()
            .RequireApiKey();

        group.MapGet("/restaurantes/{restauranteId:guid}/emails-admins", ListarEmailsAdmins)
            .WithName("ListarEmailsAdmins")
            .WithSummary("Lista os e-mails dos Admins de um restaurante (uso interno)")
            .WithDescription("Chamado pela API de Estoque pra mandar o aviso de estoque baixo. Exige o cabeçalho x-api-key; sem ele (ou errado), 401.")
            .Produces<EmailsAdminsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> ListarEmailsAdmins(Guid restauranteId, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListarEmailsAdminsQuery(restauranteId), cancellationToken);
        return Results.Ok(result);
    }
}
