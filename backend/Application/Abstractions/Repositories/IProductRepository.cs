using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Products;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? name,
        Guid? categoryId,
        ProductSortBy sortBy,
        SortDirection sortDirection,
        CancellationToken cancellationToken);

    void Add(Product product);

    void Update(Product product);

    void Remove(Product product);
}
