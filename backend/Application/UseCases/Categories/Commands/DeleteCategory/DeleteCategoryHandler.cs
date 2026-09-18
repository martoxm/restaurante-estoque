using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Application.UseCases.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _context;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, IApplicationDbContext context)
    {
        _categoryRepository = categoryRepository;
        _context = context;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            throw new NotFoundException($"Categoria com Id '{request.Id}' não encontrada.");

        if (category.Products.Any())
            throw new DomainException("Não é possível excluir uma categoria que possui produtos vinculados.");

        _categoryRepository.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
