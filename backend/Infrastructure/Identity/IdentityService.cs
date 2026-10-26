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

    public async Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        return usuario is not null;
    }

    public async Task<Guid> CriarUsuarioAsync(string nome, string email, string senha, Guid restauranteId, CancellationToken cancellationToken)
    {
        var usuario = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            Nome = nome,
            RestauranteId = restauranteId
        };

        var result = await _userManager.CreateAsync(usuario, senha);

        if (!result.Succeeded)
        {
            var failures = result.Errors
                .Select(error => new ValidationFailure(CampoDoErro(error.Code), error.Description));

            throw new ValidationException(failures);
        }

        await _userManager.AddToRoleAsync(usuario, RoleNames.Funcionario);

        return usuario.Id;
    }

    private static string CampoDoErro(string codigo)
    {
        if (codigo.StartsWith("Password", StringComparison.Ordinal))
            return "Senha";

        if (codigo.EndsWith("Email", StringComparison.Ordinal) || codigo.EndsWith("UserName", StringComparison.Ordinal))
            return "Email";

        return string.Empty;
    }

    public async Task<UsuarioAutenticado?> ValidarCredenciaisAsync(string email, string senha, CancellationToken cancellationToken)
    {
        var usuario = await _userManager.FindByEmailAsync(email);

        if (usuario is null || !await _userManager.CheckPasswordAsync(usuario, senha))
            return null;

        return new UsuarioAutenticado(usuario.Id, usuario.Nome, usuario.Email!, usuario.RestauranteId);
    }

    public async Task<IReadOnlyList<string>> ObterRolesAsync(Guid usuarioId, CancellationToken cancellationToken)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId.ToString());

        if (usuario is null)
            return [];

        var roles = await _userManager.GetRolesAsync(usuario);
        return roles.ToList();
    }

    public async Task<IReadOnlyList<string>> ListarEmailsAdminsAsync(Guid restauranteId, CancellationToken cancellationToken)
    {
        var admins = await _userManager.GetUsersInRoleAsync(RoleNames.Admin);

        return admins
            .Where(usuario => usuario.RestauranteId == restauranteId && !string.IsNullOrWhiteSpace(usuario.Email))
            .Select(usuario => usuario.Email!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
