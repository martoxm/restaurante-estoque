using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Application.UseCases.Products.Commands.CreateProduct;
using RestauranteEstoque.Application.UseCases.Products.Commands.UpdateProduct;
using RestauranteEstoque.Application.UseCases.Products.Queries.GetProductById;
using RestauranteEstoque.Application.UseCases.Products.Queries.GetProducts;
using RestauranteEstoque.Contracts.Products;

namespace RestauranteEstoque.Api.Modules;

public class ProductEndpointsModule : IEndpointModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/products")
            .WithApiVersionSet(versionSet)
            .WithTags("Products")
            .RequireAuthorization();

        group.MapPost("", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Cria um novo produto")
            .WithDescription("Cria um produto vinculado a uma categoria existente, com uma quantidade inicial opcional em estoque.")
            .Produces<ProductCreatedResponse>(StatusCodes.Status201Created);

        group.MapGet("", GetProducts)
            .WithName("GetProducts")
            .WithSummary("Lista produtos com paginação")
            .WithDescription("Lista produtos de forma paginada, com filtros opcionais por nome e categoria, e ordenação por nome, preço ou quantidade em estoque.")
            .Produces<PagedResult<ProductListItemDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Busca um produto pelo Id")
            .WithDescription("Retorna os detalhes de um produto específico, ou 404 se ele não existir.")
            .Produces<ProductDetailsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Atualiza um produto existente")
            .WithDescription("Atualiza nome, descrição, preço e categoria de um produto. Não altera a quantidade em estoque — isso é feito via movimentação de estoque. Retorna 404 se ele não existir.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateProduct(CreateProductCommand command, ISender sender)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/v1/products/{id}", new ProductCreatedResponse(id));
    }

    private static async Task<IResult> GetProducts([AsParameters] GetProductsRequest request, ISender sender)
    {
        var query = new GetProductsQuery(
            request.Page,
            request.PageSize,
            request.Name,
            request.CategoryId,
            request.SortBy,
            request.SortDirection);

        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductById(Guid id, ISender sender)
    {
        var result = await sender.Send(new GetProductByIdQuery(id));
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> UpdateProduct(Guid id, UpdateProductRequest request, ISender sender)
    {
        await sender.Send(new UpdateProductCommand(id, request.Name, request.Description, request.Price, request.CategoryId));
        return Results.NoContent();
    }
}
