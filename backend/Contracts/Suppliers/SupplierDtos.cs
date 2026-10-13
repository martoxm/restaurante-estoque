using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Contracts.Suppliers;

public record GetSuppliersRequest(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc);

public record SupplierCreatedResponse(Guid Id);

public record SupplierDetailsDto(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    DateTime CreatedAt);

public record SupplierListItemDto(
    Guid Id,
    string Name,
    string? Phone,
    string? Email,
    DateTime CreatedAt);

public record UpdateSupplierRequest(string Name, string? Phone = null, string? Email = null);
