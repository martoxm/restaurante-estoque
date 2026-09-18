using MediatR;
using RestauranteEstoque.Contracts.Products;

namespace RestauranteEstoque.Application.UseCases.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailsDto?>;
