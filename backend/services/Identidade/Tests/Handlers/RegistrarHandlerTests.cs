using FluentValidation;
using FluentValidation.Results;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Identidade.Application.UseCases.Auth;
using RestauranteEstoque.Identidade.Tests.Common;

namespace RestauranteEstoque.Identidade.Tests.Handlers;

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
        Assert.Equal("rt-1", resposta.RefreshToken);
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
        new(
            identityService,
            new EmissorSessao(identityService, new FakeJwtTokenGenerator(), new FakeGeradorRefreshToken(), new FakeRefreshTokenRepository(context), new FakeRelogio()),
            new FakeRestauranteRepository(context),
            context,
            new FakeRelogio());
}
