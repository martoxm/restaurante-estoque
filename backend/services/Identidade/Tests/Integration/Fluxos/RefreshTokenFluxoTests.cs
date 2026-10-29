using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Identidade.Application.Abstractions.Repositories;
using RestauranteEstoque.Identidade.Application.UseCases.Auth;
using RestauranteEstoque.Identidade.Domain.Entities;
using RestauranteEstoque.Identidade.Infrastructure.Persistence;
using RestauranteEstoque.Identidade.Tests.Integration.Fixtures;

namespace RestauranteEstoque.Identidade.Tests.Integration.Fluxos;

[Collection(nameof(SqlServerCollection))]
public sealed class RefreshTokenFluxoTests(SqlServerFixture fixture)
{
    [DockerFact]
    public async Task Renovar_DeveGirarOToken()
    {
        if (!fixture.EstaDisponivel)
            return;

        await using var servicos = fixture.CriarServicos();

        async Task<TResposta> EnviarAsync<TResposta>(IRequest<TResposta> comando)
        {
            using var scope = servicos.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<ISender>().Send(comando);
        }

        var email = $"rt-{Guid.NewGuid():N}@teste.com";
        var cadastro = await EnviarAsync(new RegistrarCommand("Ana", email, "senha123", "Restaurante RT"));

        var renovado = await EnviarAsync(new RenovarTokenCommand(cadastro.RefreshToken));

        Assert.NotEqual(cadastro.RefreshToken, renovado.RefreshToken);
        Assert.False(string.IsNullOrWhiteSpace(renovado.Token));

        var deNovo = await EnviarAsync(new RenovarTokenCommand(renovado.RefreshToken));
        Assert.NotEqual(renovado.RefreshToken, deNovo.RefreshToken);
    }

    [DockerFact]
    public async Task Renovar_ComTokenInventado_DeveRecusar()
    {
        if (!fixture.EstaDisponivel)
            return;

        await using var servicos = fixture.CriarServicos();
        using var scope = servicos.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await Assert.ThrowsAsync<SessaoInvalidaException>(() => sender.Send(new RenovarTokenCommand("inventado")));
    }

    [DockerFact]
    public async Task RemoverVencidos_DeveApagarSoOQueNaoServeMais()
    {
        if (!fixture.EstaDisponivel)
            return;

        var agora = new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var usuarioId = Guid.NewGuid();
        var vencido = RefreshToken.Criar(usuarioId, Hash(), agora.AddDays(-20), agora.AddDays(-6));
        var revogadoAntigo = RefreshToken.Criar(usuarioId, Hash(), agora.AddDays(-3), agora.AddDays(11));
        revogadoAntigo.Revogar(agora.AddDays(-2));
        var revogadoRecente = RefreshToken.Criar(usuarioId, Hash(), agora.AddHours(-1), agora.AddDays(13));
        revogadoRecente.Revogar(agora.AddMinutes(-10));
        var ativo = RefreshToken.Criar(usuarioId, Hash(), agora, agora.AddDays(14));

        await using (var escrita = fixture.CriarContexto())
        {
            escrita.RefreshTokens.AddRange(vencido, revogadoAntigo, revogadoRecente, ativo);
            await escrita.SaveChangesAsync();
        }

        await using (var contexto = fixture.CriarContexto())
        {
            var repository = new Infrastructure.Persistence.Repositories.RefreshTokenRepository(contexto);
            await repository.RemoverVencidosAsync(agora, CancellationToken.None);
        }

        await using var leitura = fixture.CriarContexto();
        var sobraram = await leitura.RefreshTokens
            .Where(token => token.UsuarioId == usuarioId)
            .Select(token => token.Id)
            .ToListAsync();

        Assert.Equal(2, sobraram.Count);
        Assert.Contains(revogadoRecente.Id, sobraram);
        Assert.Contains(ativo.Id, sobraram);
    }

    private static string Hash() => Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
}
