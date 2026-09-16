using MediatR;

namespace RestauranteEstoque.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest;
