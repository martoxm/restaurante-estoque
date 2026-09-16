using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Application.Suppliers.Commands.CreateSupplier;
using RestauranteEstoque.Application.Suppliers.Commands.DeleteSupplier;
using RestauranteEstoque.Application.Suppliers.Commands.UpdateSupplier;
using RestauranteEstoque.Application.Suppliers.Queries.GetSupplierById;
using RestauranteEstoque.Application.Suppliers.Queries.GetSuppliers;
using RestauranteEstoque.Contracts.Suppliers;

namespace RestauranteEstoque.Api.Modules;

public class SupplierEndpointsModule : IEndpointModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/suppliers")
            .WithApiVersionSet(versionSet)
            .WithTags("Suppliers")
            .RequireAuthorization();

        group.MapPost("", CreateSupplier)
            .WithName("CreateSupplier")
            .WithSummary("Cria um novo fornecedor")
            .WithDescription("Cria um fornecedor.")
            .Produces<SupplierCreatedResponse>(StatusCodes.Status201Created);

        group.MapGet("", GetSuppliers)
            .WithName("GetSuppliers")
            .WithSummary("Lista fornecedores com paginação")
            .WithDescription("Lista fornecedores de forma paginada, com filtro opcional por nome e ordenação por nome (crescente ou decrescente).")
            .Produces<PagedResult<SupplierListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetSupplierById)
            .WithName("GetSupplierById")
            .WithSummary("Busca um fornecedor pelo Id")
            .WithDescription("Retorna os detalhes de um fornecedor específico, ou 404 se ele não existir.")
            .Produces<SupplierDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateSupplier)
            .WithName("UpdateSupplier")
            .WithSummary("Atualiza um fornecedor existente")
            .WithDescription("Atualiza o nome, telefone e e-mail de um fornecedor. Retorna 404 se ele não existir.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteSupplier)
            .WithName("DeleteSupplier")
            .WithSummary("Exclui um fornecedor")
            .WithDescription("Exclui um fornecedor. Retorna 404 se ele não existir.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateSupplier(CreateSupplierCommand command, ISender sender)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/v1/suppliers/{id}", new SupplierCreatedResponse(id));
    }

    private static async Task<IResult> GetSuppliers([AsParameters] GetSuppliersRequest request, ISender sender)
    {
        var query = new GetSuppliersQuery(request.Page, request.PageSize, request.Name, request.SortDirection);
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetSupplierById(Guid id, ISender sender)
    {
        var result = await sender.Send(new GetSupplierByIdQuery(id));
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateSupplier(Guid id, UpdateSupplierRequest request, ISender sender)
    {
        await sender.Send(new UpdateSupplierCommand(id, request.Name, request.Phone, request.Email));
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteSupplier(Guid id, ISender sender)
    {
        await sender.Send(new DeleteSupplierCommand(id));
        return Results.NoContent();
    }
}
