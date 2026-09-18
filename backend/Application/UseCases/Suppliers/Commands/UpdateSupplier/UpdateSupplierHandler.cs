using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierHandler : IRequestHandler<UpdateSupplierCommand>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IApplicationDbContext _context;

    public UpdateSupplierHandler(ISupplierRepository supplierRepository, IApplicationDbContext context)
    {
        _supplierRepository = supplierRepository;
        _context = context;
    }

    public async Task Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            throw new NotFoundException($"Fornecedor com Id '{request.Id}' não encontrado.");

        supplier.Update(request.Name, request.Phone, request.Email);

        _supplierRepository.Update(supplier);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
