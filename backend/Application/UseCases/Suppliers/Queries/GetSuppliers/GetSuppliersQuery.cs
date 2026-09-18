using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Suppliers;

namespace RestauranteEstoque.Application.UseCases.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc) : IRequest<PagedResult<SupplierListItemDto>>;
