namespace RestauranteEstoque.Core.Mensageria.Contratos;


public sealed record ProcessarPagamentoMensagem(Guid VendaId, Guid RestauranteId, decimal Valor, string TokenCartao);

public sealed record ResultadoPagamentoMensagem(Guid VendaId, Guid RestauranteId, Guid PagamentoId, bool Aprovado, string? Motivo);

public sealed record DevolverEstoqueMensagem(Guid VendaId, Guid RestauranteId, IReadOnlyList<ItemBaixaEstoque> Itens, string Motivo);
