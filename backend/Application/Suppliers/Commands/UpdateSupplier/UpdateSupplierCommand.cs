using MediatR;

namespace RestauranteEstoque.Application.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand(
    Guid Id,
    string Name,
    string? Phone,
    string? Email) : IRequest;
