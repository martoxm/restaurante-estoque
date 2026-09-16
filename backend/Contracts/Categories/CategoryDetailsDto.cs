namespace RestauranteEstoque.Contracts.Categories;

public record CategoryDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    int ProductCount);
