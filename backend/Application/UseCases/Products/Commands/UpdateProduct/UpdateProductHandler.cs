using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;

namespace RestauranteEstoque.Application.UseCases.Products.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IApplicationDbContext _context;

    public UpdateProductHandler(IProductRepository productRepository, IApplicationDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
            throw new NotFoundException($"Produto com Id '{request.Id}' não encontrado.");

        product.Update(request.Name, request.Description, request.Price, request.CategoryId);

        _productRepository.Update(product);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
