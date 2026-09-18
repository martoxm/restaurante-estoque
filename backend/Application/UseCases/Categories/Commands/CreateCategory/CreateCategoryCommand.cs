using MediatR;

namespace RestauranteEstoque.Application.UseCases.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description) : IRequest<Guid>;
