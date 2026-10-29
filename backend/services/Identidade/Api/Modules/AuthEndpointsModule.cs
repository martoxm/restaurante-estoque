using RestauranteEstoque.Core.Api;
using Asp.Versioning;
using MediatR;
using RestauranteEstoque.Identidade.Application.UseCases.Auth;
using RestauranteEstoque.Identidade.Contracts.Auth;

namespace RestauranteEstoque.Identidade.Api.Modules;

public class AuthEndpointsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
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

        group.MapPost("/registrar", Registrar)
            .WithName("Registrar")
            .WithSummary("Cadastra um novo usuário")
            .WithDescription("Cria um usuário novo (nome, e-mail e senha) e já devolve um token JWT, exatamente como o /login — não é preciso logar de novo depois de se cadastrar.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/renovar", Renovar)
            .WithName("RenovarToken")
            .WithSummary("Renova a sessão")
            .WithDescription("Troca um refresh token válido por um token de acesso novo (15 min) e um refresh token novo (14 dias). O refresh token enviado é revogado na hora: cada um só serve uma vez.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Renovar(RenovarTokenCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Login(LoginCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Registrar(RegistrarCommand command, ISender sender, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
}
