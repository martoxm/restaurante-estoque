using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Suppliers;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Supplier> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? name,
        SortDirection sortDirection,
        CancellationToken cancellationToken);

    void Add(Supplier supplier);

    void Update(Supplier supplier);

    void Remove(Supplier supplier);
}
