using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Application.UseCases.Produtos;
using RestauranteEstoque.Contracts.Auth;
using RestauranteEstoque.Contracts.Produtos;

namespace RestauranteEstoque.Api.Modules;

public class ProdutoEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/produtos")
            .WithApiVersionSet(versionSet)
            .WithTags("Produtos")
            .RequireAuthorization();

        group.MapPost("", CriarProduto)
            .WithName("CriarProduto")
            .WithSummary("Cria um novo produto")
            .WithDescription("Cria um produto vinculado a uma categoria existente, com uma quantidade inicial opcional em estoque.")
            .Produces<ProdutoCriadoResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("", ListarProdutos)
            .WithName("ListarProdutos")
            .WithSummary("Lista produtos com paginação")
            .WithDescription("Lista produtos de forma paginada, com filtros opcionais por nome e categoria, e ordenação por nome, preço ou quantidade em estoque.")
            .Produces<ResultadoPaginado<ProdutoItemListaDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", ObterProdutoPorId)
            .WithName("ObterProdutoPorId")
            .WithSummary("Busca um produto pelo Id")
            .WithDescription("Retorna os detalhes de um produto específico, ou 404 se ele não existir.")
            .Produces<ProdutoDetalhesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", AtualizarProduto)
            .WithName("AtualizarProduto")
            .WithSummary("Atualiza um produto existente")
            .WithDescription("Atualiza nome, descrição, preço e categoria de um produto. Não altera a quantidade em estoque — isso é feito via movimentação de estoque. Retorna 404 se ele não existir.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", ExcluirProduto)
            .WithName("ExcluirProduto")
            .WithSummary("Exclui um produto")
            .WithDescription("Exclui um produto, desde que ele não tenha movimentações de estoque vinculadas. Requer a role Admin. Retorna 404 se ele não existir, e 400 se ainda houver movimentações vinculadas.")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CriarProduto(CriarProdutoCommand command, ISender sender)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/v1/produtos/{id}", new ProdutoCriadoResponse(id));
    }

    private static async Task<IResult> ListarProdutos([AsParameters] ListarProdutosRequest request, ISender sender)
    {
        var query = new ListarProdutosQuery(
            request.Pagina,
            request.TamanhoPagina,
            request.Nome,
            request.CategoriaId,
            request.OrdenarPor,
            request.DirecaoOrdenacao);

        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> ObterProdutoPorId(Guid id, ISender sender)
    {
        var result = await sender.Send(new ObterProdutoPorIdQuery(id));
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> AtualizarProduto(Guid id, AtualizarProdutoRequest request, ISender sender)
    {
        await sender.Send(new AtualizarProdutoCommand(id, request.Nome, request.Descricao, request.Preco, request.CategoriaId));
        return Results.NoContent();
    }

    private static async Task<IResult> ExcluirProduto(Guid id, ISender sender)
    {
        await sender.Send(new ExcluirProdutoCommand(id));
        return Results.NoContent();
    }
}
