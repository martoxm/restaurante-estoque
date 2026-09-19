using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Application.UseCases.Products.Commands.DeleteProduct;

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
