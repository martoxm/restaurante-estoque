using MediatR;
using RestauranteEstoque.Application.Common.Interfaces;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IApplicationDbContext _context;

    public CreateProductHandler(IProductRepository productRepository, IApplicationDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.InitialQuantity);

        _productRepository.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
