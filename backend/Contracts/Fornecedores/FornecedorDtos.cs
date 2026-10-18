using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Fornecedores;

public record ListarFornecedoresRequest(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc);

public record FornecedorCriadoResponse(Guid Id);

public record FornecedorDetalhesDto(
    Guid Id,
    string Nome,
    string? Telefone,
    string? Email,
    DateTime CriadoEm);

public record FornecedorItemListaDto(
    Guid Id,
    string Nome,
    string? Telefone,
    string? Email,
    DateTime CriadoEm);

public record AtualizarFornecedorRequest(string Nome, string? Telefone = null, string? Email = null);
