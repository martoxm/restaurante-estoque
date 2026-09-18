using MediatR;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Commands.DeleteSupplier;

public record DeleteSupplierCommand(Guid Id) : IRequest;
