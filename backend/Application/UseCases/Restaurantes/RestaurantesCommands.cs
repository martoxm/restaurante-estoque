using MediatR;

namespace RestauranteEstoque.Application.UseCases.Restaurantes;

public record AtualizarRestauranteCommand(Guid Id, string Nome) : IRequest;
