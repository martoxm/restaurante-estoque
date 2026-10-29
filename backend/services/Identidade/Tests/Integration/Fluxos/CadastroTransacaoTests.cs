using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Identidade.Application.UseCases.Auth;
using RestauranteEstoque.Identidade.Tests.Integration.Fixtures;

namespace RestauranteEstoque.Identidade.Tests.Integration.Fluxos;

[Collection(nameof(SqlServerCollection))]
public sealed class CadastroTransacaoTests(SqlServerFixture fixture)
{
    [DockerFact]
    public async Task Registrar_ComSenhaRecusadaPeloIdentity_NaoDeveDeixarRestauranteOrfao()
    {
        if (!fixture.EstaDisponivel)
            return;

        var nomeRestaurante = NomeUnico("Restaurante órfão");
        var email = EmailUnico();

        await Assert.ThrowsAsync<ValidationException>(() =>
            EnviarAsync(new RegistrarCommand("Fulano", email, "123", nomeRestaurante)));

        await using var contexto = fixture.CriarContexto();
        Assert.Equal(0, await contexto.Restaurantes.IgnoreQueryFilters().CountAsync(r => r.Nome == nomeRestaurante));
        Assert.False(await contexto.Users.AnyAsync(usuario => usuario.Email == email));
    }

    [DockerFact]
    public async Task Registrar_ComSenhaValida_DeveGravarRestauranteEUsuarioJuntos()
    {
        if (!fixture.EstaDisponivel)
            return;

        var nomeRestaurante = NomeUnico("Restaurante novo");
        var email = EmailUnico();

        await EnviarAsync(new RegistrarCommand("Fulano", email, "senha123", nomeRestaurante));

        await using var contexto = fixture.CriarContexto();
        var restaurante = await contexto.Restaurantes.IgnoreQueryFilters().SingleAsync(r => r.Nome == nomeRestaurante);
        var usuario = await contexto.Users.SingleAsync(u => u.Email == email);
        Assert.Equal(restaurante.Id, usuario.RestauranteId);
    }

    private async Task<T> EnviarAsync<T>(IRequest<T> comando)
    {
        await using var servicos = fixture.CriarServicos();
        using var escopo = servicos.CreateScope();
        var sender = escopo.ServiceProvider.GetRequiredService<ISender>();

        return await sender.Send(comando, CancellationToken.None);
    }

    private static string NomeUnico(string prefixo) => $"{prefixo} {Guid.NewGuid():N}";

    private static string EmailUnico() => $"teste-{Guid.NewGuid():N}@exemplo.com";
}
