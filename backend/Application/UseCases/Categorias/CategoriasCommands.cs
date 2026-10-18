using MediatR;

namespace RestauranteEstoque.Application.UseCases.Categorias;

public record CriarCategoriaCommand(
    string Nome,
    string? Descricao) : IRequest<Guid>;

public record ExcluirCategoriaCommand(Guid Id) : IRequest;

public record AtualizarCategoriaCommand(
    Guid Id,
    string Nome,
    string? Descricao) : IRequest;
