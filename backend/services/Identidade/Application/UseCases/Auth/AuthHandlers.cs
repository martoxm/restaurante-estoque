using MediatR;
using RestauranteEstoque.Identidade.Application.Abstractions.Repositories;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Core.Application.Exceptions;
using RestauranteEstoque.Identidade.Contracts.Auth;
using RestauranteEstoque.Identidade.Domain.Entities;

namespace RestauranteEstoque.Identidade.Application.UseCases.Auth;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly EmissorSessao _emissorSessao;
    private readonly IApplicationDbContext _context;

    public LoginHandler(IIdentityService identityService, EmissorSessao emissorSessao, IApplicationDbContext context)
    {
        _identityService = identityService;
        _emissorSessao = emissorSessao;
        _context = context;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var usuario = await _identityService.ValidarCredenciaisAsync(email, request.Senha, cancellationToken) ?? throw new CredenciaisInvalidasException();

        var resposta = await _emissorSessao.EmitirAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return resposta;
    }
}

public class RegistrarHandler : IRequestHandler<RegistrarCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly EmissorSessao _emissorSessao;
    private readonly IRestauranteRepository _restauranteRepository;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;

    public RegistrarHandler(
        IIdentityService identityService,
        EmissorSessao emissorSessao,
        IRestauranteRepository restauranteRepository,
        IApplicationDbContext context,
        IRelogio relogio)
    {
        _identityService = identityService;
        _emissorSessao = emissorSessao;
        _restauranteRepository = restauranteRepository;
        _context = context;
        _relogio = relogio;
    }

    public async Task<LoginResponse> Handle(RegistrarCommand request, CancellationToken cancellationToken)
    {
        await using var transacao = await _context.IniciarTransacaoAsync(cancellationToken);

        var restaurante = Restaurante.Criar(request.NomeRestaurante, _relogio.AgoraUtc);
        _restauranteRepository.Adicionar(restaurante);
        await _context.SaveChangesAsync(cancellationToken);

        var usuarioId = await _identityService.CriarUsuarioAsync(request.Nome, request.Email, request.Senha, restaurante.Id, cancellationToken);

        var usuario = new UsuarioAutenticado(usuarioId, request.Nome, request.Email, restaurante.Id);
        var resposta = await _emissorSessao.EmitirAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await transacao.ConfirmarAsync(cancellationToken);

        return resposta;
    }
}

public class RenovarTokenHandler : IRequestHandler<RenovarTokenCommand, LoginResponse>
{
    private const string MensagemSessaoExpirada = "Sua sessão expirou. Faça login novamente.";

    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IGeradorRefreshToken _geradorRefreshToken;
    private readonly IIdentityService _identityService;
    private readonly EmissorSessao _emissorSessao;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;

    public RenovarTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IGeradorRefreshToken geradorRefreshToken,
        IIdentityService identityService,
        EmissorSessao emissorSessao,
        IApplicationDbContext context,
        IRelogio relogio)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _geradorRefreshToken = geradorRefreshToken;
        _identityService = identityService;
        _emissorSessao = emissorSessao;
        _context = context;
        _relogio = relogio;
    }

    public async Task<LoginResponse> Handle(RenovarTokenCommand request, CancellationToken cancellationToken)
    {
        var agora = _relogio.AgoraUtc;
        var hash = _geradorRefreshToken.CalcularHash(request.RefreshToken);

        var refreshToken = await _refreshTokenRepository.ObterPorHashAsync(hash, cancellationToken)
            ?? throw new SessaoInvalidaException(MensagemSessaoExpirada);

        if (refreshToken.FoiReusadoForaDaTolerancia(agora))
        {
            var ativos = await _refreshTokenRepository.ListarAtivosDoUsuarioAsync(refreshToken.UsuarioId, agora, cancellationToken);
            foreach (var ativo in ativos)
                _refreshTokenRepository.Remover(ativo);

            await _context.SaveChangesAsync(cancellationToken);
            throw new SessaoInvalidaException(MensagemSessaoExpirada);
        }

        if (!refreshToken.PodeSerUsado(agora))
            throw new SessaoInvalidaException(MensagemSessaoExpirada);

        var usuario = await _identityService.ObterUsuarioPorIdAsync(refreshToken.UsuarioId, cancellationToken)
            ?? throw new SessaoInvalidaException(MensagemSessaoExpirada);

        refreshToken.Revogar(agora);
        _refreshTokenRepository.Atualizar(refreshToken);

        var resposta = await _emissorSessao.EmitirAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return resposta;
    }
}

public class EmissorSessao
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IGeradorRefreshToken _geradorRefreshToken;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRelogio _relogio;

    public EmissorSessao(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator,
        IGeradorRefreshToken geradorRefreshToken,
        IRefreshTokenRepository refreshTokenRepository,
        IRelogio relogio)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _geradorRefreshToken = geradorRefreshToken;
        _refreshTokenRepository = refreshTokenRepository;
        _relogio = relogio;
    }

    public async Task<LoginResponse> EmitirAsync(UsuarioAutenticado usuario, CancellationToken cancellationToken)
    {
        var roles = await _identityService.ObterRolesAsync(usuario.Id, cancellationToken);
        var token = _jwtTokenGenerator.GerarToken(usuario.Id, usuario.Nome, usuario.Email, roles, usuario.RestauranteId);

        var agora = _relogio.AgoraUtc;
        var refreshToken = _geradorRefreshToken.Gerar();
        _refreshTokenRepository.Adicionar(
            RefreshToken.Criar(usuario.Id, refreshToken.Hash, agora, agora.AddDays(_geradorRefreshToken.DiasValidade)));

        return new LoginResponse(token, refreshToken.Token, usuario.Nome, usuario.Email);
    }
}
