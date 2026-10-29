using MediatR;
using RestauranteEstoque.Core.Application.Models;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Produtos;

namespace RestauranteEstoque.Application.UseCases.Produtos;

public record ObterProdutoPorIdQuery(Guid Id) : IRequest<ProdutoDetalhesDto?>;

public record ListarProdutosQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    Guid? CategoriaId = null,
    OrdenacaoProduto OrdenarPor = OrdenacaoProduto.Nome,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc) : IRequest<ResultadoPaginado<ProdutoItemListaDto>>;
