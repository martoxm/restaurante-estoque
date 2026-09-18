using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Categories;
using RestauranteEstoque.Contracts.Common;

namespace RestauranteEstoque.Application.UseCases.Categories.Queries.GetCategories;

public record GetCategoriesQuery(
    int Page = 1,
    int PageSize = 10,
    string? Name = null,
    SortDirection SortDirection = SortDirection.Asc) : IRequest<PagedResult<CategoryListItemDto>>;
