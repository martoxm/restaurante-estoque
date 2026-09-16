namespace RestauranteEstoque.Contracts.Suppliers;

public record SupplierDetailsDto(
    Guid Id,
    string Name,
    string? Phone,
    string? Email);
