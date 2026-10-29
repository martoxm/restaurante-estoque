using RestauranteEstoque.Identidade.Contracts.Common;

namespace RestauranteEstoque.Identidade.Contracts.Restaurantes;

public record ListarRestaurantesRequest(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Nome = null,
    DirecaoOrdenacao DirecaoOrdenacao = DirecaoOrdenacao.Asc);

public record RestauranteDetalhesDto(Guid Id, string Nome, DateTime CriadoEm);

public record RestauranteItemListaDto(Guid Id, string Nome, DateTime CriadoEm);

public record AtualizarRestauranteRequest(string Nome);

public record EmailsAdminsDto(IReadOnlyList<string> Emails);
