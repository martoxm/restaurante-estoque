using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Domain.Events;

public sealed record EstoqueFicouBaixoEvent(
    Guid ProdutoId,
    string NomeProduto,
    int QuantidadeAtual,
    Guid RestauranteId) : IDomainEvent;
