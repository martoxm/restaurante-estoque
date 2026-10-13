using MediatR;

namespace RestauranteEstoque.Application.UseCases.Categories;

public record CreateCategoryCommand(
    string Name,
    string? Description) : IRequest<Guid>;

public record DeleteCategoryCommand(Guid Id) : IRequest;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description) : IRequest;
