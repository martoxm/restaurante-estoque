namespace RestauranteEstoque.Contracts.Categories;

public record CategoryListItemDto(
    Guid Id,
    string Name,
    string? Description,
    int ProductCount);
