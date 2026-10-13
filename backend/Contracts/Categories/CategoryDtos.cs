using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Categories;

public record CategoryCreatedResponse(Guid Id);

public record CategoryDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    int ProductCount,
    DateTime CreatedAt);

public record CategoryListItemDto(
    Guid Id,
    string Name,
    string? Description,
    int ProductCount,
    DateTime CreatedAt);

public record GetCategoriesRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc);

public record UpdateCategoryRequest(string Name, string? Description = null);
