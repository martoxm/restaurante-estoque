using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Core.Api;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Vendas.Application.UseCases.Vendas;
using RestauranteEstoque.Vendas.Contracts.Vendas;

namespace RestauranteEstoque.Vendas.Api.Modules;

public class VendaEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/vendas")
            .WithApiVersionSet(versionSet)
            .WithTags("Vendas")
            .RequireAuthorization();

        group.MapPost("", CriarVenda)
            .WithName("CriarVenda")
            .WithSummary("Registra uma nova venda")
            .WithDescription("Cria uma venda com status Pendente. Nome e preço de cada item vêm do catálogo local (não do corpo da requisição). Depois o Estoque baixa os itens (AguardandoPagamento) e a Pagamentos cobra pelo tokenCartao: aprovado vira Confirmada; sem estoque ou pagamento recusado vira Cancelada (com o estoque devolvido). Tokens de teste: tok_aprovado, tok_saldo_insuficiente, tok_fraude, tok_gateway_fora. Retorna 404 se algum produto não existir.")
            .Produces<VendaCriadaResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("", ListarVendas)
            .WithName("ListarVendas")
            .WithSummary("Lista vendas com paginação")
            .WithDescription("Lista as vendas do restaurante, ordenadas pela data de criação (por padrão, as mais novas primeiro).")
            .Produces<ResultadoPaginado<VendaItemListaDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", ObterVendaPorId)
            .WithName("ObterVendaPorId")
            .WithSummary("Busca uma venda pelo Id")
            .WithDescription("Retorna a venda com os itens, ou 404 se ela não existir.")
            .Produces<VendaDetalhesDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> CriarVenda(CriarVendaCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/v1/vendas/{id}", new VendaCriadaResponse(id));
    }

    private static async Task<IResult> ListarVendas([AsParameters] ListarVendasRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var query = new ListarVendasQuery(request.Pagina, request.TamanhoPagina, request.DirecaoOrdenacao);
        var result = await sender.Send(query, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ObterVendaPorId(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ObterVendaPorIdQuery(id), cancellationToken);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }
}
