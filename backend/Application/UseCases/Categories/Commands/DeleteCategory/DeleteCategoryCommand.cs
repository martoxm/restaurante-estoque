using MediatR;

namespace RestauranteEstoque.Application.UseCases.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest;
