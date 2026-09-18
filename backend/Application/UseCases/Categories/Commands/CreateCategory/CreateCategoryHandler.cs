using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Categories.Commands.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _context;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, IApplicationDbContext context)
    {
        _categoryRepository = categoryRepository;
        _context = context;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name, request.Description);

        _categoryRepository.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
