namespace RestauranteEstoque.Contracts.Categories;

public record UpdateCategoryRequest(string Name, string? Description = null);
