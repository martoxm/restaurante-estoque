using MediatR;

namespace RestauranteEstoque.Application.UseCases.Suppliers;

public record CreateSupplierCommand(
    string Name,
    string? Phone,
    string? Email) : IRequest<Guid>;

public record DeleteSupplierCommand(Guid Id) : IRequest;

public record UpdateSupplierCommand(
    Guid Id,
    string Name,
    string? Phone,
    string? Email) : IRequest;
