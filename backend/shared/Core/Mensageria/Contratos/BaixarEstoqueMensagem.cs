namespace RestauranteEstoque.Core.Mensageria.Contratos;

public sealed record BaixarEstoqueMensagem(Guid VendaId, Guid RestauranteId, IReadOnlyList<ItemBaixaEstoque> Itens);

public sealed record ItemBaixaEstoque(Guid ProdutoId, int Quantidade);
