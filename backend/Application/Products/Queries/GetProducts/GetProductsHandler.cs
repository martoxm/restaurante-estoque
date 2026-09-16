using MediatR;
using RestauranteEstoque.Application.Common.Models;
using RestauranteEstoque.Contracts.Products;

namespace RestauranteEstoque.Application.Products.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Name,
            request.CategoryId,
            request.SortBy,
            request.SortDirection,
            cancellationToken);

        var items = products
            .Select(product => new ProductListItemDto(
                product.Id,
                product.Name,
                product.Price,
                product.QuantityInStock,
                product.CategoryId,
                product.Category!.Name))
            .ToList();

        return new PagedResult<ProductListItemDto>(items, request.Page, request.PageSize, totalCount);
    }
}
