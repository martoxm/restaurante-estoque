namespace RestauranteEstoque.Contracts.Suppliers;

public record SupplierListItemDto(
    Guid Id,
    string Name,
    string? Phone,
    string? Email);
