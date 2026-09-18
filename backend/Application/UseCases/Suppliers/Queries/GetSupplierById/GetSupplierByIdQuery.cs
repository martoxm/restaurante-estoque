using MediatR;
using RestauranteEstoque.Contracts.Suppliers;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Queries.GetSupplierById;

public record GetSupplierByIdQuery(Guid Id) : IRequest<SupplierDetailsDto?>;
