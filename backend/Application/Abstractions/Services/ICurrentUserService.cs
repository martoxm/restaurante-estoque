namespace RestauranteEstoque.Application.Abstractions.Services;

public interface ICurrentUserService
{
    Guid? RestaurantId { get; }
}
