using MediatR;
using RestauranteEstoque.Contracts.Categories;

namespace RestauranteEstoque.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsDto?>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDetailsDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            return null;

        return new CategoryDetailsDto(
            category.Id,
            category.Name,
            category.Description,
            category.Products.Count);
    }
}
