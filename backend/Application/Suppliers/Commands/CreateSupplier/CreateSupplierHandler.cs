using MediatR;
using RestauranteEstoque.Application.Common.Interfaces;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierHandler : IRequestHandler<CreateSupplierCommand, Guid>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IApplicationDbContext _context;

    public CreateSupplierHandler(ISupplierRepository supplierRepository, IApplicationDbContext context)
    {
        _supplierRepository = supplierRepository;
        _context = context;
    }

    public async Task<Guid> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = Supplier.Create(request.Name, request.Phone, request.Email);

        _supplierRepository.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
