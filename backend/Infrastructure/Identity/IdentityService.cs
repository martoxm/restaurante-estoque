using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<Guid> CreateUserAsync(string name, string email, string password, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            Name = name
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var failures = result.Errors
                .Select(error => new ValidationFailure(nameof(password), error.Description));

            throw new ValidationException(failures);
        }

        await _userManager.AddToRoleAsync(user, RoleNames.Funcionario);

        return user.Id;
    }

    public async Task<AuthenticatedUser?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
            return null;

        return new AuthenticatedUser(user.Id, user.Name, user.Email!);
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return [];

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}
