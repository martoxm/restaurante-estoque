using MediatR;
using RestauranteEstoque.Contracts.Categories;

namespace RestauranteEstoque.Application.UseCases.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDetailsDto?>;
