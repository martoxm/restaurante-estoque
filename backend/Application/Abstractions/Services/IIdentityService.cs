namespace RestauranteEstoque.Application.Abstractions.Services;

public record AuthenticatedUser(Guid Id, string Name, string Email);

public interface IIdentityService
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    Task<Guid> CreateUserAsync(string name, string email, string password, CancellationToken cancellationToken);

    Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken);
}
