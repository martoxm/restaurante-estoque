using MediatR;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Application.Common.Exceptions;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.UseCases.Auth.Commands.Login;

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

        var user = await _identityService.ValidateCredentialsAsync(email, request.Password, cancellationToken);

        if (user is null)
            throw new InvalidCredentialsException();

        var roles = await _identityService.GetRolesAsync(user.Id, cancellationToken);
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Name, user.Email, roles);

        return new LoginResponse(token, user.Name, user.Email);
    }
}
