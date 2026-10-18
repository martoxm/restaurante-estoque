using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Categorias;

public record CategoriaCriadaResponse(Guid Id);

public record CategoriaDetalhesDto(
    Guid Id,
    string Nome,
    string? Descricao,
    int QuantidadeProdutos,
    DateTime CriadoEm);

public record CategoriaItemListaDto(
    Guid Id,
    string Nome,
    string? Descricao,
    int QuantidadeProdutos,
    DateTime CriadoEm);

public record ListarCategoriasRequest(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc);

public record AtualizarCategoriaRequest(string Nome, string? Descricao = null);
