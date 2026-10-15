using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Application.UseCases.Restaurants;
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

        group.MapPost("", CreateRestaurant)
            .WithName("CreateRestaurant")
            .WithSummary("Cria um novo restaurante")
            .WithDescription("Cria um restaurante (tenant do sistema multi-restaurante).")
            .Produces<RestaurantCreatedResponse>(StatusCodes.Status201Created);

        group.MapGet("", GetRestaurants)
            .WithName("GetRestaurants")
            .WithSummary("Lista restaurantes com paginação")
            .WithDescription("Lista restaurantes de forma paginada, com filtro opcional por nome e ordenação por nome (crescente ou decrescente).")
            .Produces<PagedResult<RestaurantListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetRestaurantById)
            .WithName("GetRestaurantById")
            .WithSummary("Busca um restaurante pelo Id")
            .WithDescription("Retorna os detalhes de um restaurante específico, ou 404 se ele não existir.")
            .Produces<RestaurantDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateRestaurant)
            .WithName("UpdateRestaurant")
            .WithSummary("Atualiza um restaurante existente")
            .WithDescription("Atualiza o nome de um restaurante. Retorna 404 se ele não existir.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteRestaurant)
            .WithName("DeleteRestaurant")
            .WithSummary("Exclui um restaurante")
            .WithDescription("Exclui um restaurante. Retorna 404 se ele não existir.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CreateRestaurant(CreateRestaurantCommand command, ISender sender)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/v1/restaurants/{id}", new RestaurantCreatedResponse(id));
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

    private static async Task<IResult> DeleteRestaurant(Guid id, ISender sender)
    {
        await sender.Send(new DeleteRestaurantCommand(id));
        return Results.NoContent();
    }
}
