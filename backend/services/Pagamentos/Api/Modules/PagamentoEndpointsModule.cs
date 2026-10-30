using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Core.Api;
using RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;
using RestauranteEstoque.Pagamentos.Contracts.Pagamentos;

namespace RestauranteEstoque.Pagamentos.Api.Modules;

public class PagamentoEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/pagamentos")
            .WithApiVersionSet(versionSet)
            .WithTags("Pagamentos")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", ObterPagamentoPorId)
            .WithName("ObterPagamentoPorId")
            .WithSummary("Consulta um pagamento")
            .WithDescription("Mostra o status (Pendente, Aprovado ou Recusado), o Id da transação no gateway e o motivo da recusa, se houver.")
            .Produces<PagamentoDetalhesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/venda/{vendaId:guid}", ObterPagamentoPorVenda)
            .WithName("ObterPagamentoPorVenda")
            .WithSummary("Consulta o pagamento de uma venda")
            .WithDescription("Devolve o pagamento da venda, ou 404 se a cobrança ainda não foi pedida (a venda está Pendente, esperando o estoque).")
            .Produces<PagamentoDetalhesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ObterPagamentoPorId(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ObterPagamentoPorIdQuery(id), cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> ObterPagamentoPorVenda(Guid vendaId, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ObterPagamentoPorVendaQuery(vendaId), cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }
}
