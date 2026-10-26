using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Application.UseCases.Restaurantes;
using RestauranteEstoque.Contracts.Auth;
using RestauranteEstoque.Contracts.Restaurantes;

namespace RestauranteEstoque.Api.Modules;

public class RestauranteEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/restaurantes")
            .WithApiVersionSet(versionSet)
            .WithTags("Restaurantes")
            .RequireAuthorization();

        group.MapGet("", ListarRestaurantes)
            .WithName("ListarRestaurantes")
            .WithSummary("Lista restaurantes com paginação")
            .WithDescription("Lista os restaurantes visíveis ao usuário logado (na prática, só o dele), de forma paginada, com filtro opcional por nome e ordenação por nome.")
            .Produces<ResultadoPaginado<RestauranteItemListaDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", ObterRestaurantePorId)
            .WithName("ObterRestaurantePorId")
            .WithSummary("Busca um restaurante pelo Id")
            .WithDescription("Retorna os detalhes do restaurante do usuário logado, ou 404 se o Id não existir ou for de outro restaurante.")
            .Produces<RestauranteDetalhesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", AtualizarRestaurante)
            .WithName("AtualizarRestaurante")
            .WithSummary("Atualiza um restaurante existente")
            .WithDescription("Atualiza o nome do restaurante do usuário logado. Exige a role Admin. Retorna 404 se o Id for de outro restaurante.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin));
    }

    private static async Task<IResult> ListarRestaurantes([AsParameters] ListarRestaurantesRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var query = new ListarRestaurantesQuery(request.Pagina, request.TamanhoPagina, request.Nome, request.DirecaoOrdenacao);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ObterRestaurantePorId(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ObterRestaurantePorIdQuery(id), cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> AtualizarRestaurante(Guid id, AtualizarRestauranteRequest request, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new AtualizarRestauranteCommand(id, request.Nome), cancellationToken);
        return Results.NoContent();
    }
}
