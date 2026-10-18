using RestauranteEstoque.Contracts.Common;
using System.Text.Json.Serialization;

namespace RestauranteEstoque.Contracts.Produtos;

public record ListarProdutosRequest(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    Guid? CategoriaId = null,
    OrdenacaoProduto OrdenarPor = OrdenacaoProduto.Nome,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc);

public record ProdutoCriadoResponse(Guid Id);

public record ProdutoDetalhesDto(
    Guid Id,
    string Nome,
    string? Descricao,
    decimal Preco,
    int QuantidadeEmEstoque,
    Guid CategoriaId,
    string NomeCategoria,
    DateTime CriadoEm);

public record ProdutoItemListaDto(
    Guid Id,
    string Nome,
    decimal Preco,
    int QuantidadeEmEstoque,
    Guid CategoriaId,
    string NomeCategoria,
    DateTime CriadoEm);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrdenacaoProduto
{
    Nome,
    Preco,
    QuantidadeEmEstoque
}

public record AtualizarProdutoRequest(
    string Nome,
    string? Descricao,
    decimal Preco,
    Guid CategoriaId);
