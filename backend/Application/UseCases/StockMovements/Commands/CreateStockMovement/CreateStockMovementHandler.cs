using MediatR;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Domain.Entities;
using RestauranteEstoque.Domain.Enums;

namespace RestauranteEstoque.Application.UseCases.StockMovements.Commands.CreateStockMovement;

public class CreateStockMovementHandler : IRequestHandler<CreateStockMovementCommand, Guid>
{
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IProductRepository _productRepository;
    private readonly IApplicationDbContext _context;
    private readonly IClock _clock;

    public CreateStockMovementHandler(
        IStockMovementRepository stockMovementRepository,
        IProductRepository productRepository,
        IApplicationDbContext context,
        IClock clock)
    {
        _stockMovementRepository = stockMovementRepository;
        _productRepository = productRepository;
        _context = context;
        _clock = clock;
    }

    public async Task<Guid> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
            throw new NotFoundException($"Produto com Id '{request.ProductId}' não foi encontrado.");

        var movement = StockMovement.Create(request.ProductId, request.Type, request.Quantity, _clock.UtcNow, request.Notes);

        if (request.Type == StockMovementType.Entrada)
            product.IncreaseStock(request.Quantity);
        else
            product.DecreaseStock(request.Quantity);

        _productRepository.Update(product);
        _stockMovementRepository.Add(movement);
        await _context.SaveChangesAsync(cancellationToken);

        return movement.Id;
    }
}
