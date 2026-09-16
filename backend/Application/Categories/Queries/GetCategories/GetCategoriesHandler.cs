using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Categories;

namespace RestauranteEstoque.Application.Categories.Queries.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, PagedResult<CategoryListItemDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<CategoryListItemDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var (categories, totalCount) = await _categoryRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Name,
            request.SortDirection,
            cancellationToken);

        var items = categories
            .Select(category => new CategoryListItemDto(
                category.Id,
                category.Name,
                category.Description,
                category.Products.Count))
            .ToList();

        return new PagedResult<CategoryListItemDto>(items, request.Page, request.PageSize, totalCount);
    }
}
