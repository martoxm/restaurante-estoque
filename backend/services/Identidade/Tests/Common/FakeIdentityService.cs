using RestauranteEstoque.Identidade.Application.Abstractions.Services;

namespace RestauranteEstoque.Identidade.Tests.Common;

public class FakeIdentityService : IIdentityService
{
    public Exception? ErroAoCriar { get; init; }

    public Dictionary<Guid, UsuarioAutenticado> Usuarios { get; } = [];

    public Task<Guid> CriarUsuarioAsync(string nome, string email, string senha, Guid restauranteId, CancellationToken cancellationToken)
    {
        if (ErroAoCriar is not null)
            return Task.FromException<Guid>(ErroAoCriar);

        var id = Guid.NewGuid();
        Usuarios[id] = new UsuarioAutenticado(id, nome, email, restauranteId);
        return Task.FromResult(id);
    }

    public Task<UsuarioAutenticado?> ObterUsuarioPorIdAsync(Guid usuarioId, CancellationToken cancellationToken) =>
        Task.FromResult(Usuarios.GetValueOrDefault(usuarioId));

    public Task<IReadOnlyList<string>> ObterRolesAsync(Guid usuarioId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<string>>(["Funcionario"]);

    public Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<UsuarioAutenticado?> ValidarCredenciaisAsync(string email, string senha, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Dictionary<Guid, IReadOnlyList<string>> EmailsAdmins { get; } = [];

    public Task<IReadOnlyList<string>> ListarEmailsAdminsAsync(Guid restauranteId, CancellationToken cancellationToken)
        => Task.FromResult(EmailsAdmins.TryGetValue(restauranteId, out var emails) ? emails : (IReadOnlyList<string>)[]);
}

public class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public string GerarToken(Guid usuarioId, string nome, string email, IEnumerable<string> roles, Guid? restauranteId) => "token-falso";
}

public class FakeGeradorRefreshToken : IGeradorRefreshToken
{
    private int _contador;

    public int DiasValidade => 14;

    public RefreshTokenGerado Gerar()
    {
        var token = $"rt-{++_contador}";
        return new RefreshTokenGerado(token, CalcularHash(token));
    }

    public string CalcularHash(string token) => $"hash-{token}";
}
