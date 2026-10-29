using MediatR;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Fornecedores;

namespace RestauranteEstoque.Application.UseCases.Fornecedores;

public record ObterFornecedorPorIdQuery(Guid Id) : IRequest<FornecedorDetalhesDto?>;

public record ListarFornecedoresQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc) : IRequest<ResultadoPaginado<FornecedorItemListaDto>>;
