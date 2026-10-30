using MediatR;
using RestauranteEstoque.Pagamentos.Contracts.Pagamentos;

namespace RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;

public record ObterPagamentoPorIdQuery(Guid Id) : IRequest<PagamentoDetalhesDto?>;

public record ObterPagamentoPorVendaQuery(Guid VendaId) : IRequest<PagamentoDetalhesDto?>;
