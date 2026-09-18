namespace RestauranteEstoque.Application.Abstractions.Services;

public interface IClock
{
    DateTime UtcNow { get; }
}
