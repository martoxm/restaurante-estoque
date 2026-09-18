using MediatR;

namespace RestauranteEstoque.Application.UseCases.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description) : IRequest;
