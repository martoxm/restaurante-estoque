using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.UseCases.Categorias;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Auth;
using RestauranteEstoque.Contracts.Categorias;

namespace RestauranteEstoque.Api.Modules;

public class CategoriaEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/categorias")
            .WithApiVersionSet(versionSet)
            .WithTags("Categorias")
            .RequireAuthorization();

        group.MapPost("", CriarCategoria)
            .WithName("CriarCategoria")
            .WithSummary("Cria uma nova categoria")
            .WithDescription("Cria uma categoria de produtos.")
            .Produces<CategoriaCriadaResponse>(StatusCodes.Status201Created);

        group.MapGet("", ListarCategorias)
            .WithName("ListarCategorias")
            .WithSummary("Lista categorias com paginação")
            .WithDescription("Lista categorias de forma paginada, com filtro opcional por nome e ordenação por nome (crescente ou decrescente).")
            .Produces<ResultadoPaginado<CategoriaItemListaDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", ObterCategoriaPorId)
            .WithName("ObterCategoriaPorId")
            .WithSummary("Busca uma categoria pelo Id")
            .WithDescription("Retorna os detalhes de uma categoria específica, ou 404 se ela não existir.")
            .Produces<CategoriaDetalhesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", AtualizarCategoria)
            .WithName("AtualizarCategoria")
            .WithSummary("Atualiza uma categoria existente")
            .WithDescription("Atualiza o nome e a descrição de uma categoria. Requer a role Admin. Retorna 404 se ela não existir.")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", ExcluirCategoria)
            .WithName("ExcluirCategoria")
            .WithSummary("Exclui uma categoria")
            .WithDescription("Exclui uma categoria, desde que ela não tenha produtos vinculados. Requer a role Admin. Retorna 404 se ela não existir, e 400 se ainda houver produtos vinculados.")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CriarCategoria(CriarCategoriaCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/v1/categorias/{id}", new CategoriaCriadaResponse(id));
    }

    private static async Task<IResult> ListarCategorias([AsParameters] ListarCategoriasRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var query = new ListarCategoriasQuery(request.Pagina, request.TamanhoPagina, request.Nome, request.DirecaoOrdenacao);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ObterCategoriaPorId(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ObterCategoriaPorIdQuery(id), cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> AtualizarCategoria(Guid id, AtualizarCategoriaRequest request, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new AtualizarCategoriaCommand(id, request.Nome, request.Descricao), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ExcluirCategoria(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new ExcluirCategoriaCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
