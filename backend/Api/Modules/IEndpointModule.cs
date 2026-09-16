namespace RestauranteEstoque.Api.Modules;

public interface IEndpointModule
{
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app);
}
