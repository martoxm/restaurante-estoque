using FluentValidation;
using FluentValidation.Results;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.UseCases.Auth;
using RestauranteEstoque.Tests.Common;

namespace RestauranteEstoque.Tests.Handlers;

public class RegistrarHandlerTests
{
    [Fact]
    public async Task Handle_ComDadosValidos_DeveConfirmarATransacaoEDevolverToken()
    {
        using var context = TestDbContextFactory.Create();
        var handler = CriarHandler(context, new FakeIdentityService());

        var resposta = await handler.Handle(
            new RegistrarCommand("Ana", "ana@teste.com", "senha123", "Restaurante da Ana"), CancellationToken.None);

        Assert.Equal("token-falso", resposta.Token);
        Assert.True(context.UltimaTransacao!.Confirmada);
        Assert.True(context.UltimaTransacao.Descartada);
    }

    [Fact]
    public async Task Handle_QuandoOIdentityRecusaOUsuario_NaoDeveConfirmarATransacao()
    {
        using var context = TestDbContextFactory.Create();
        var identityService = new FakeIdentityService { ErroAoCriar = new ValidationException([new ValidationFailure("Senha", "Senha fraca.")]) };
        var handler = CriarHandler(context, identityService);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new RegistrarCommand("Ana", "ana@teste.com", "123", "Restaurante da Ana"), CancellationToken.None));

        Assert.False(context.UltimaTransacao!.Confirmada);
        Assert.True(context.UltimaTransacao.Descartada);
    }

    private static RegistrarHandler CriarHandler(TestDbContext context, IIdentityService identityService) =>
        new(identityService, new FakeJwtTokenGenerator(), new FakeRestauranteRepository(context), context, new FakeRelogio());

    private sealed class FakeIdentityService : IIdentityService
    {
        public Exception? ErroAoCriar { get; init; }

        public Task<Guid> CriarUsuarioAsync(string nome, string email, string senha, Guid restauranteId, CancellationToken cancellationToken) =>
            ErroAoCriar is null ? Task.FromResult(Guid.NewGuid()) : Task.FromException<Guid>(ErroAoCriar);

        public Task<IReadOnlyList<string>> ObterRolesAsync(Guid usuarioId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<string>>(["Funcionario"]);

        public Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task<UsuarioAutenticado?> ValidarCredenciaisAsync(string email, string senha, CancellationToken cancellationToken) => throw new NotImplementedException();

        public Task<IReadOnlyList<string>> ListarEmailsAdminsAsync(Guid restauranteId, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        public string GerarToken(Guid usuarioId, string nome, string email, IEnumerable<string> roles, Guid? restauranteId) => "token-falso";
    }
}
