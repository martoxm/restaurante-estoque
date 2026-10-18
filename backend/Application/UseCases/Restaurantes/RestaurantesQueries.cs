using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Restaurantes;

namespace RestauranteEstoque.Application.UseCases.Restaurantes;

public record ObterRestaurantePorIdQuery(Guid Id) : IRequest<RestauranteDetalhesDto?>;

public record ListarRestaurantesQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc) : IRequest<ResultadoPaginado<RestauranteItemListaDto>>;
