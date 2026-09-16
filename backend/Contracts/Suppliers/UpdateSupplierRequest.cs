namespace RestauranteEstoque.Contracts.Suppliers;

public record UpdateSupplierRequest(string Name, string? Phone = null, string? Email = null);
