using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.StockMovements.Commands.CreateStockMovement;
using RestauranteEstoque.Contracts.StockMovements;

namespace RestauranteEstoque.Api.Modules;

public class StockMovementEndpointsModule : IEndpointModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/stock-movements")
            .WithApiVersionSet(versionSet)
            .WithTags("StockMovements")
            .RequireAuthorization();

        group.MapPost("", CreateStockMovement)
            .WithName("CreateStockMovement")
            .WithSummary("Registra uma movimentação de estoque")
            .WithDescription("Registra uma entrada ou saída de estoque para um produto existente, atualizando a quantidade em estoque dele. Retorna 404 se o produto não existir, e 400 se a saída pedida for maior que o estoque disponível.")
            .Produces<StockMovementCreatedResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> CreateStockMovement(CreateStockMovementCommand command, ISender sender)
    {
        var id = await sender.Send(command);
        return Results.Created($"/api/v1/stock-movements/{id}", new StockMovementCreatedResponse(id));
    }
}
