using MediatR;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand(
    Guid Id,
    string Name,
    string? Phone,
    string? Email) : IRequest;
