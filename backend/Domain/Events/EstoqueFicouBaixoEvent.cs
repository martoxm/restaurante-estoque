using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Domain.Events;

public sealed record EstoqueFicouBaixoEvent(
    Guid ProdutoId,
    string NomeProduto,
    int QuantidadeAtual,
    Guid RestauranteId) : IDomainEvent;
