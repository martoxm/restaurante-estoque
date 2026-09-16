using MediatR;
using RestauranteEstoque.Contracts.Products;

namespace RestauranteEstoque.Application.Products.Queries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDetailsDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDetailsDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
            return null;

        return new ProductDetailsDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.QuantityInStock,
            product.CategoryId,
            product.Category!.Name);
    }
}
