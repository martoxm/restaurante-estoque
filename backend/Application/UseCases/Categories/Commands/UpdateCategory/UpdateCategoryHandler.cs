using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;

namespace RestauranteEstoque.Application.UseCases.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _context;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IApplicationDbContext context)
    {
        _categoryRepository = categoryRepository;
        _context = context;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException($"Categoria com Id '{request.Id}' não encontrada.");

        category.Update(request.Name, request.Description);

        _categoryRepository.Update(category);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
