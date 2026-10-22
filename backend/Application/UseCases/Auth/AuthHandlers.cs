using MediatR;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Contracts.Auth;
using RestauranteEstoque.Domain.Entities;

namespace RestauranteEstoque.Application.UseCases.Auth;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginHandler(IIdentityService identityService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var usuario = await _identityService.ValidarCredenciaisAsync(email, request.Senha, cancellationToken);

        if (usuario is null)
            throw new CredenciaisInvalidasException();

        var roles = await _identityService.ObterRolesAsync(usuario.Id, cancellationToken);
        var token = _jwtTokenGenerator.GerarToken(usuario.Id, usuario.Nome, usuario.Email, roles, usuario.RestauranteId);

        return new LoginResponse(token, usuario.Nome, usuario.Email);
    }
}

public class RegistrarHandler : IRequestHandler<RegistrarCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRestauranteRepository _restauranteRepository;
    private readonly IApplicationDbContext _context;
    private readonly IRelogio _relogio;

    public RegistrarHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator,
        IRestauranteRepository restauranteRepository,
        IApplicationDbContext context,
        IRelogio relogio)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
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

        await transacao.ConfirmarAsync(cancellationToken);
        var roles = await _identityService.ObterRolesAsync(usuarioId, cancellationToken);

        var token = _jwtTokenGenerator.GerarToken(usuarioId, request.Nome, request.Email, roles, restaurante.Id);

        return new LoginResponse(token, request.Nome, request.Email);
    }
}
