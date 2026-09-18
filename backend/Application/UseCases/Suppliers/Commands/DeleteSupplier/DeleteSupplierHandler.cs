using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierHandler : IRequestHandler<DeleteSupplierCommand>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IApplicationDbContext _context;

    public DeleteSupplierHandler(ISupplierRepository supplierRepository, IApplicationDbContext context)
    {
        _supplierRepository = supplierRepository;
        _context = context;
    }

    public async Task Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            throw new NotFoundException($"Fornecedor com Id '{request.Id}' não encontrado.");

        _supplierRepository.Remove(supplier);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
