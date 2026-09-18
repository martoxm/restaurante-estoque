using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Abstractions.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
