using RestauranteEstoque.Core.Api;
using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Application.UseCases.Fornecedores;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Contracts.Fornecedores;

namespace RestauranteEstoque.Api.Modules;

public class FornecedorEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/fornecedores")
            .WithApiVersionSet(versionSet)
            .WithTags("Fornecedores")
            .RequireAuthorization();

        group.MapPost("", CriarFornecedor)
            .WithName("CriarFornecedor")
            .WithSummary("Cria um novo fornecedor")
            .WithDescription("Cria um fornecedor.")
            .Produces<FornecedorCriadoResponse>(StatusCodes.Status201Created);

        group.MapGet("", ListarFornecedores)
            .WithName("ListarFornecedores")
            .WithSummary("Lista fornecedores com paginação")
            .WithDescription("Lista fornecedores de forma paginada, com filtro opcional por nome e ordenação por nome (crescente ou decrescente).")
            .Produces<ResultadoPaginado<FornecedorItemListaDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", ObterFornecedorPorId)
            .WithName("ObterFornecedorPorId")
            .WithSummary("Busca um fornecedor pelo Id")
            .WithDescription("Retorna os detalhes de um fornecedor específico, ou 404 se ele não existir.")
            .Produces<FornecedorDetalhesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", AtualizarFornecedor)
            .WithName("AtualizarFornecedor")
            .WithSummary("Atualiza um fornecedor existente")
            .WithDescription("Atualiza o nome, telefone e e-mail de um fornecedor. Requer a role Admin. Retorna 404 se ele não existir.")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", ExcluirFornecedor)
            .WithName("ExcluirFornecedor")
            .WithSummary("Exclui um fornecedor")
            .WithDescription("Exclui um fornecedor. Requer a role Admin. Retorna 404 se ele não existir.")
            .RequireAuthorization(policy => policy.RequireRole(RoleNames.Admin))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CriarFornecedor(CriarFornecedorCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/v1/fornecedores/{id}", new FornecedorCriadoResponse(id));
    }

    private static async Task<IResult> ListarFornecedores([AsParameters] ListarFornecedoresRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var query = new ListarFornecedoresQuery(request.Pagina, request.TamanhoPagina, request.Nome, request.DirecaoOrdenacao);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ObterFornecedorPorId(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ObterFornecedorPorIdQuery(id), cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> AtualizarFornecedor(Guid id, AtualizarFornecedorRequest request, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new AtualizarFornecedorCommand(id, request.Nome, request.Telefone, request.Email), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> ExcluirFornecedor(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new ExcluirFornecedorCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
