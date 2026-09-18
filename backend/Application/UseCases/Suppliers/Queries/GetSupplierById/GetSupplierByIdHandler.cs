using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Contracts.Suppliers;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Queries.GetSupplierById;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDetailsDto?>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSupplierByIdHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDetailsDto?> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _supplierRepository.GetByIdAsync(request.Id, cancellationToken);

        if (supplier is null)
            return null;

        return new SupplierDetailsDto(
            supplier.Id,
            supplier.Name,
            supplier.Phone,
            supplier.Email);
    }
}
