using MediatR;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Application.UseCases.MovimentacoesEstoque;

public record CriarMovimentacaoEstoqueCommand(
    Guid ProdutoId,
    TipoMovimentacaoEstoque Tipo,
    int Quantidade,
    string? Observacoes = null) : IRequest<Guid>;
