using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Categories;
using RestauranteEstoque.Domain.Common;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Categories;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IClock _clock;

    public CreateCategoryHandler(
        ICategoryRepository categoryRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IClock clock)
    {
        _categoryRepository = categoryRepository;
        _context = context;
        _currentUserService = currentUserService;
        _clock = clock;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name, _clock.UtcNow, request.Description, _currentUserService.RestaurantId);

        _categoryRepository.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}

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
                category.Products.Count,
                category.CreatedAt))
            .ToList();

        return new PagedResult<CategoryListItemDto>(items, request.Page, request.PageSize, totalCount);
    }
}

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
            category.Products.Count,
            category.CreatedAt);
    }
}
