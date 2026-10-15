using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Application.UseCases.Restaurants;
using RestauranteEstoque.Contracts.Auth;
using RestauranteEstoque.Contracts.Restaurants;

namespace RestauranteEstoque.Api.Modules;

public class RestaurantEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/restaurants")
            .WithApiVersionSet(versionSet)
            .WithTags("Restaurants")
            .RequireAuthorization();

        group.MapGet("", GetRestaurants)
            .WithName("GetRestaurants")
            .WithSummary("Lista restaurantes com paginação")
            .WithDescription("Lista os restaurantes visíveis ao usuário logado (na prática, só o dele), de forma paginada, com filtro opcional por nome e ordenação por nome.")
            .Produces<PagedResult<RestaurantListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetRestaurantById)
            .WithName("GetRestaurantById")
            .WithSummary("Busca um restaurante pelo Id")
            .WithDescription("Retorna os detalhes do restaurante do usuário logado, ou 404 se o Id não existir ou for de outro restaurante.")
            .Produces<RestaurantDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateRestaurant)
            .WithName("UpdateRestaurant")
            .WithSummary("Atualiza um restaurante existente")
            .WithDescription("Atualiza o nome do restaurante do usuário logado. Exige a role Admin. Retorna 404 se o Id for de outro restaurante.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin));
    }

    private static async Task<IResult> GetRestaurants([AsParameters] GetRestaurantsRequest request, ISender sender)
    {
        var query = new GetRestaurantsQuery(request.Page, request.PageSize, request.Name, request.SortDirection);
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetRestaurantById(Guid id, ISender sender)
    {
        var result = await sender.Send(new GetRestaurantByIdQuery(id));
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateRestaurant(Guid id, UpdateRestaurantRequest request, ISender sender)
    {
        await sender.Send(new UpdateRestaurantCommand(id, request.Name));
        return Results.NoContent();
    }
}
