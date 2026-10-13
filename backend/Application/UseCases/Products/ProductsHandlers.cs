using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Products;
using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Products;

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

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IApplicationDbContext _context;

    public DeleteProductHandler(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IApplicationDbContext context)
    {
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _context = context;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
            throw new NotFoundException($"Produto com Id '{request.Id}' não encontrado.");

        var hasStockMovements = await _stockMovementRepository.ExistsForProductAsync(request.Id, cancellationToken);

        if (hasStockMovements)
            throw new DomainException("Não é possível excluir um produto que possui movimentações de estoque vinculadas.");

        _productRepository.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

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
            product.Category!.Name,
            product.CreatedAt);
    }
}

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Name,
            request.CategoryId,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var items = products
            .Select(product => new ProductListItemDto(
                product.Id,
                product.Name,
                product.Price,
                product.QuantityInStock,
                product.CategoryId,
                product.Category!.Name,
                product.CreatedAt))
            .ToList();

        return new PagedResult<ProductListItemDto>(items, request.Page, request.PageSize, totalCount);
    }
}
