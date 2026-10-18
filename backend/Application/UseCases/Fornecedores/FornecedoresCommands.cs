using MediatR;

namespace RestauranteEstoque.Application.UseCases.Fornecedores;

public record CriarFornecedorCommand(
    string Nome,
    string? Telefone,
    string? Email) : IRequest<Guid>;

public record ExcluirFornecedorCommand(Guid Id) : IRequest;

public record AtualizarFornecedorCommand(
    Guid Id,
    string Nome,
    string? Telefone,
    string? Email) : IRequest;
