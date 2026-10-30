using MediatR;
using RestauranteEstoque.Core.Mensageria.Contratos;
using RestauranteEstoque.Vendas.Contracts.Vendas;

namespace RestauranteEstoque.Vendas.Application.UseCases.Vendas;

public record CriarVendaCommand(IReadOnlyList<ItemVendaRequest> Itens) : IRequest<Guid>;

public record ProcessarResultadoBaixaCommand(Guid MensagemId, ResultadoBaixaEstoqueMensagem Resultado) : IRequest;
