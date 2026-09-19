using MediatR;

namespace RestauranteEstoque.Application.UseCases.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;
