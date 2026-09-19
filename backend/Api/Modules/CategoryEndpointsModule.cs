using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.UseCases.Categories.Commands.CreateCategory;
using RestauranteEstoque.Application.UseCases.Categories.Commands.DeleteCategory;
using RestauranteEstoque.Application.UseCases.Categories.Commands.UpdateCategory;
using RestauranteEstoque.Application.UseCases.Categories.Queries.GetCategories;
using RestauranteEstoque.Application.UseCases.Categories.Queries.GetCategoryById;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Auth;
using RestauranteEstoque.Contracts.Categories;

namespace RestauranteEstoque.Api.Modules;

public class CategoryEndpointsModule : IEndpointModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/categories")
            .WithApiVersionSet(versionSet)
            .WithTags("Categories")
            .RequireAuthorization();

        group.MapPost("", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Cria uma nova categoria")
            .WithDescription("Cria uma categoria de produtos.")
            .Produces<CategoryCreatedResponse>(StatusCodes.Status201Created);

        group.MapGet("", GetCategories)
            .WithName("GetCategories")
            .WithSummary("Lista categorias com paginação")
            .WithDescription("Lista categorias de forma paginada, com filtro opcional por nome e ordenação por nome (crescente ou decrescente).")
            .Produces<PagedResult<CategoryListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetCategoryById)
            .WithName("GetCategoryById")
            .WithSummary("Busca uma categoria pelo Id")
            .WithDescription("Retorna os detalhes de uma categoria específica, ou 404 se ela não existir.")
            .Produces<CategoryDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithSummary("Atualiza uma categoria existente")
            .WithDescription("Atualiza o nome e a descrição de uma categoria. Requer a role Admin. Retorna 404 se ela não existir.")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithSummary("Exclui uma categoria")
            .WithDescription("Exclui uma categoria, desde que ela não tenha produtos vinculados. Requer a role Admin. Retorna 404 se ela não existir, e 400 se ainda houver produtos vinculados.")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateCategory(CreateCategoryCommand command, ISender sender)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/v1/categories/{id}", new CategoryCreatedResponse(id));
    }

    private static async Task<IResult> GetCategories([AsParameters] GetCategoriesRequest request, ISender sender)
    {
        var query = new GetCategoriesQuery(request.Page, request.PageSize, request.Name, request.SortDirection);
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCategoryById(Guid id, ISender sender)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id));
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateCategory(Guid id, UpdateCategoryRequest request, ISender sender)
    {
        await sender.Send(new UpdateCategoryCommand(id, request.Name, request.Description));
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteCategory(Guid id, ISender sender)
    {
        await sender.Send(new DeleteCategoryCommand(id));
        return Results.NoContent();
    }
}
