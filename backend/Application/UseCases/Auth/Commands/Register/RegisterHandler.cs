using MediatR;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Application.UseCases.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterHandler(IIdentityService identityService, IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userId = await _identityService.CreateUserAsync(request.Name, request.Email, request.Password, cancellationToken);
        var roles = await _identityService.GetRolesAsync(userId, cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(userId, request.Name, request.Email, roles);

        return new LoginResponse(token, request.Name, request.Email);
    }
}
