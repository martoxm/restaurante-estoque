using MediatR;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(
    string Name,
    string? Phone,
    string? Email) : IRequest<Guid>;
