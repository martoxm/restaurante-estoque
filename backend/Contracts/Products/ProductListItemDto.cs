namespace RestauranteEstoque.Contracts.Products;

public record ProductListItemDto(
    Guid Id,
    string Name,
    decimal Price,
    int QuantityInStock,
    Guid CategoryId,
    string CategoryName);
