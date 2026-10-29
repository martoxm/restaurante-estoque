using MediatR;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Identidade.Contracts.Common;
using RestauranteEstoque.Identidade.Contracts.Restaurantes;

namespace RestauranteEstoque.Identidade.Application.UseCases.Restaurantes;

public record ObterRestaurantePorIdQuery(Guid Id) : IRequest<RestauranteDetalhesDto?>;

public record ListarRestaurantesQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc) : IRequest<ResultadoPaginado<RestauranteItemListaDto>>;

public record ListarEmailsAdminsQuery(Guid RestauranteId) : IRequest<EmailsAdminsDto>;
