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

        var user = await _identityService.ValidateCredentialsAsync(email, request.Password, cancellationToken);

        if (user is null)
            throw new InvalidCredentialsException();

        var roles = await _identityService.GetRolesAsync(user.Id, cancellationToken);
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Name, user.Email, roles, user.RestaurantId);

        return new LoginResponse(token, user.Name, user.Email);
    }
}

public class RegisterHandler : IRequestHandler<RegisterCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IApplicationDbContext _context;
    private readonly IClock _clock;

    public RegisterHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator,
        IRestaurantRepository restaurantRepository,
        IApplicationDbContext context,
        IClock clock)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _restaurantRepository = restaurantRepository;
        _context = context;
        _clock = clock;
    }

    public async Task<LoginResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var restaurant = Restaurant.Create(request.RestaurantName, _clock.UtcNow);
        _restaurantRepository.Add(restaurant);
        await _context.SaveChangesAsync(cancellationToken);

        var userId = await _identityService.CreateUserAsync(request.Name, request.Email, request.Password, restaurant.Id, cancellationToken);
        var roles = await _identityService.GetRolesAsync(userId, cancellationToken);

        var token = _jwtTokenGenerator.GenerateToken(userId, request.Name, request.Email, roles, restaurant.Id);

        return new LoginResponse(token, request.Name, request.Email);
    }
}
