using MediatR;

namespace RestauranteEstoque.Application.UseCases.Products;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    int InitialQuantity = 0) : IRequest<Guid>;

public record DeleteProductCommand(Guid Id) : IRequest;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId) : IRequest;
