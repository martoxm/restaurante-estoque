using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Application.Auth.Commands.Login;
using RestauranteEstoque.Application.Auth.Commands.Register;
using RestauranteEstoque.Contracts.Auth;

namespace RestauranteEstoque.Api.Modules;

public class AuthEndpointsModule : IEndpointModule
{
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/auth")
            .WithApiVersionSet(versionSet)
            .WithTags("Auth");

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Autentica um usuário")
            .WithDescription("Valida e-mail e senha e devolve um token JWT, que deve ser enviado no cabeçalho \"Authorization: Bearer {token}\" dos demais endpoints protegidos.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Cadastra um novo usuário")
            .WithDescription("Cria um usuário novo (nome, e-mail e senha) e já devolve um token JWT, exatamente como o /login — não é preciso logar de novo depois de se cadastrar.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> Login(LoginCommand command, ISender sender)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }

    private static async Task<IResult> Register(RegisterCommand command, ISender sender)
    {
        var result = await sender.Send(command);
        return Results.Ok(result);
    }
}
