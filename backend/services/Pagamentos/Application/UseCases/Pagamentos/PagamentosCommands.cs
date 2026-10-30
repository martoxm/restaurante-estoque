using MediatR;
using RestauranteEstoque.Pagamentos.Contracts.Pagamentos;

namespace RestauranteEstoque.Pagamentos.Application.UseCases.Pagamentos;

public record CriarPagamentoCommand(string ChaveIdempotencia, Guid VendaId, decimal Valor, string TokenCartao)
    : IRequest<PagamentoCriadoResponse>;

public record ProcessarPagamentoCommand(Guid PagamentoId) : IRequest;

public record ProcessarWebhookGatewayCommand(
    Guid PagamentoId,
    Guid RestauranteId,
    string IdTransacao,
    bool Aprovado,
    string? Motivo) : IRequest;
