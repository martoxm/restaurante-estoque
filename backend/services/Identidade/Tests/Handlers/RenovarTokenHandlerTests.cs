using Microsoft.EntityFrameworkCore;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Identidade.Application.UseCases.Auth;
using RestauranteEstoque.Identidade.Tests.Common;

namespace RestauranteEstoque.Identidade.Tests.Handlers;

public class RenovarTokenHandlerTests
{
    private readonly TestDbContext _context = TestDbContextFactory.Create();
    private readonly FakeIdentityService _identityService = new();
    private readonly FakeGeradorRefreshToken _gerador = new();
    private readonly FakeRelogio _relogio = new();
    private readonly EmissorSessao _emissor;
    private readonly RenovarTokenHandler _handler;

    public RenovarTokenHandlerTests()
    {
        var repository = new FakeRefreshTokenRepository(_context);
        _emissor = new EmissorSessao(_identityService, new FakeJwtTokenGenerator(), _gerador, repository, _relogio);
        _handler = new RenovarTokenHandler(repository, _gerador, _identityService, _emissor, _context, _relogio);
    }

    private async Task<string> LogarAsync()
    {
        var usuarioId = await _identityService.CriarUsuarioAsync("Ana", "ana@teste.com", "senha123", Guid.NewGuid(), CancellationToken.None);
        var resposta = await _emissor.EmitirAsync(_identityService.Usuarios[usuarioId], CancellationToken.None);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        return resposta.RefreshToken;
    }

    private Task<Contracts.Auth.LoginResponse> RenovarAsync(string refreshToken)
    {
        _context.ChangeTracker.Clear();
        return _handler.Handle(new RenovarTokenCommand(refreshToken), CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ComTokenValido_DeveDevolverParNovoERevogarOAntigo()
    {
        var primeiro = await LogarAsync();

        var resposta = await RenovarAsync(primeiro);

        Assert.Equal("rt-2", resposta.RefreshToken);
        var antigo = await _context.RefreshTokens.SingleAsync(token => token.TokenHash == "hash-rt-1");
        Assert.Equal(_relogio.AgoraUtc, antigo.RevogadoEm);
    }

    [Fact]
    public async Task Handle_ComTokenDesconhecido_DeveLancarSessaoInvalida()
    {
        await Assert.ThrowsAsync<SessaoInvalidaException>(() => RenovarAsync("nao-existe"));
    }

    [Fact]
    public async Task Handle_ComTokenExpirado_DeveLancarSessaoInvalida()
    {
        var primeiro = await LogarAsync();
        _relogio.AgoraUtc = _relogio.AgoraUtc.AddDays(15);

        await Assert.ThrowsAsync<SessaoInvalidaException>(() => RenovarAsync(primeiro));
    }

    [Fact]
    public async Task Handle_ReusoDentroDaTolerancia_DeveAceitar()
    {
        var primeiro = await LogarAsync();
        await RenovarAsync(primeiro);
        _relogio.AgoraUtc = _relogio.AgoraUtc.AddSeconds(30);

        var resposta = await RenovarAsync(primeiro);

        Assert.Equal("rt-3", resposta.RefreshToken);
    }

    [Fact]
    public async Task Handle_ReusoForaDaTolerancia_DeveDerrubarTodasAsSessoes()
    {
        var primeiro = await LogarAsync();
        var segundo = (await RenovarAsync(primeiro)).RefreshToken;
        _relogio.AgoraUtc = _relogio.AgoraUtc.AddMinutes(5);

        await Assert.ThrowsAsync<SessaoInvalidaException>(() => RenovarAsync(primeiro));

        await Assert.ThrowsAsync<SessaoInvalidaException>(() => RenovarAsync(segundo));
    }

    [Fact]
    public async Task Handle_UsuarioApagado_DeveLancarSessaoInvalida()
    {
        var primeiro = await LogarAsync();
        _identityService.Usuarios.Clear();

        await Assert.ThrowsAsync<SessaoInvalidaException>(() => RenovarAsync(primeiro));
    }
}
