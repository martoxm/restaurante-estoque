using MediatR;
using RestauranteEstoque.Contracts.Categories;

namespace RestauranteEstoque.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDetailsDto?>;
