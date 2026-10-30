using MediatR;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Vendas.Contracts.Common;
using RestauranteEstoque.Vendas.Contracts.Vendas;

namespace RestauranteEstoque.Vendas.Application.UseCases.Vendas;

public record ObterVendaPorIdQuery(Guid Id) : IRequest<VendaDetalhesDto?>;

public record ListarVendasQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Desc) : IRequest<ResultadoPaginado<VendaItemListaDto>>;
