using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Common;
using RestauranteEstoque.Contracts.Products;

namespace RestauranteEstoque.Application.UseCases.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailsDto?>;

public record GetProductsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    Guid? CategoryId = null,
    ProductSortBy SortBy = ProductSortBy.Name,
    SortDirection SortDirection = SortDirection.Asc) : IRequest<PagedResult<ProductListItemDto>>;
