using MediatR;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Contracts.Categorias;
using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Application.UseCases.Categorias;

public record ListarCategoriasQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc) : IRequest<ResultadoPaginado<CategoriaItemListaDto>>;

public record ObterCategoriaPorIdQuery(Guid Id) : IRequest<CategoriaDetalhesDto?>;
