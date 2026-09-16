namespace RestauranteEstoque.Contracts.Products;

public record ProductDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int QuantityInStock,
    Guid CategoryId,
    string CategoryName);
