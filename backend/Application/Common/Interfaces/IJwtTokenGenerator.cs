using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
