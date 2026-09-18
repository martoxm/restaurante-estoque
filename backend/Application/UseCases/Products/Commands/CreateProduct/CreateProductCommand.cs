using MediatR;

namespace RestauranteEstoque.Application.UseCases.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    int InitialQuantity = 0) : IRequest<Guid>;
