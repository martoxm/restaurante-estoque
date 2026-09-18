using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Suppliers;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Queries.GetSuppliers;

public class GetSuppliersHandler : IRequestHandler<GetSuppliersQuery, PagedResult<SupplierListItemDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    public GetSuppliersHandler(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<PagedResult<SupplierListItemDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var (suppliers, totalCount) = await _supplierRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Name,
            request.SortDirection,
            cancellationToken);

        var items = suppliers
            .Select(supplier => new SupplierListItemDto(
                supplier.Id,
                supplier.Name,
                supplier.Phone,
                supplier.Email))
            .ToList();

        return new PagedResult<SupplierListItemDto>(items, request.Page, request.PageSize, totalCount);
    }
}
