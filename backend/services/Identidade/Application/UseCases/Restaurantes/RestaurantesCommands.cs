using MediatR;

namespace RestauranteEstoque.Identidade.Application.UseCases.Restaurantes;

public record AtualizarRestauranteCommand(Guid Id, string Nome) : IRequest;
