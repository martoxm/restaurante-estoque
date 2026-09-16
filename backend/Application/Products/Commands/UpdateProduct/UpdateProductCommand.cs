using MediatR;

namespace RestauranteEstoque.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId) : IRequest;
