using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Auth;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    void Add(User user);
}
