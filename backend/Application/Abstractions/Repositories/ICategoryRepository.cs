using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Abstractions.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Category> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? name,
        SortDirection sortDirection,
        CancellationToken cancellationToken);

    void Add(Category category);

    void Update(Category category);

    void Remove(Category category);
}
