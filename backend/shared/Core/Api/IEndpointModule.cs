using Microsoft.AspNetCore.Routing;
namespace RestauranteEstoque.Core.Api;

public interface IEndpointModule
{
    void MapEndpoints(IEndpointRouteBuilder app);
}
