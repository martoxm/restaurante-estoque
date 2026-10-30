using MediatR;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;

public record CriarMovimentacaoEstoqueCommand(
    Guid ProdutoId,
    TipoMovimentacaoEstoque Tipo,
    int Quantidade,
    string? Observacoes = null) : IRequest<Guid>;

public record BaixarEstoqueVendaCommand(
    Guid MensagemId,
    Guid VendaId,
    Guid RestauranteId,
    IReadOnlyList<ItemBaixaEstoque> Itens) : IRequest;
