using RestauranteEstoque.Vendas.Contracts.Common;

namespace RestauranteEstoque.Vendas.Contracts.Vendas;

public record ItemVendaRequest(Guid ProdutoId, int Quantidade);

public record VendaCriadaResponse(Guid Id);

public record ListarVendasRequest(
    int Pagina = 1,
    int TamanhoPagina = 10,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Desc);

public record ItemVendaDto(
    Guid ProdutoId,
    string NomeProduto,
    decimal PrecoUnitario,
    int Quantidade,
    decimal Subtotal);

public record VendaDetalhesDto(
    Guid Id,
    string Status,
    DateTime CriadoEm,
    decimal Total,
    IReadOnlyList<ItemVendaDto> Itens);

public record VendaItemListaDto(
    Guid Id,
    string Status,
    DateTime CriadoEm,
    int QuantidadeItens,
    decimal Total);
