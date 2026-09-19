namespace RestauranteEstoque.Application.Abstractions.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string name, string email, IEnumerable<string> roles);
}
