using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IClock _clock;

    public CreateProductHandler(
        IProductRepository productRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IClock clock)
    {
        _productRepository = productRepository;
        _context = context;
        _currentUserService = currentUserService;
        _clock = clock;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            _clock.UtcNow,
            request.InitialQuantity,
            _currentUserService.RestaurantId);

        _productRepository.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
