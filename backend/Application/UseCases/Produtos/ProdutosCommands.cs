using MediatR;

namespace RestauranteEstoque.Application.UseCases.Produtos;

public record CriarProdutoCommand(
    string Nome,
    string? Descricao,
    decimal Preco,
    Guid CategoriaId,
    int QuantidadeInicial = 0) : IRequest<Guid>;

public record ExcluirProdutoCommand(Guid Id) : IRequest;

public record AtualizarProdutoCommand(
    Guid Id,
    string Nome,
    string? Descricao,
    decimal Preco,
    Guid CategoriaId) : IRequest;
