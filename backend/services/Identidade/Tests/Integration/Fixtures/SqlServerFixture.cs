using RestauranteEstoque.Core.Application.Abstractions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Identidade.Application;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Identidade.Infrastructure;
using RestauranteEstoque.Identidade.Infrastructure.Identity;
using RestauranteEstoque.Identidade.Infrastructure.Persistence;
using RestauranteEstoque.Identidade.Tests.Common;
using Testcontainers.MsSql;

namespace RestauranteEstoque.Identidade.Tests.Integration.Fixtures;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private MsSqlContainer? _container;

    private static readonly Guid RestaurantePadrao = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public string? MotivoIndisponibilidade { get; private set; }
    public bool EstaDisponivel => MotivoIndisponibilidade is null;

    public string ConnectionString
    {
        get
        {
            GarantirDisponivel();
            return _container!.GetConnectionString();
        }
    }

    public AppDbContext CriarContexto(IUsuarioAtualService? usuarioAtualService = null)
    {
        GarantirDisponivel();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;

        return new AppDbContext(options, usuarioAtualService ?? new FakeUsuarioAtualService { RestauranteId = RestaurantePadrao });
    }

    public ServiceProvider CriarServicos(IUsuarioAtualService? usuarioAtualService = null)
    {
        GarantirDisponivel();

        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString,
                ["Jwt:Secret"] = "segredo-usado-so-nos-testes-de-integracao-com-mais-de-32-caracteres",
                ["Jwt:Issuer"] = "RestauranteEstoqueTestes",
                ["Jwt:Audience"] = "RestauranteEstoqueTestes",
                ["Jwt:ExpirationMinutes"] = "60"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddApplication();
        services.AddInfrastructure(configuracao);
        services.AddSingleton(usuarioAtualService ?? new FakeUsuarioAtualService { RestauranteId = RestaurantePadrao });

        return services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        try
        {
            _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                .Build();

            await _container.StartAsync();
        }
        catch (Exception ex) when (DockerIndisponivel(ex))
        {
            MotivoIndisponibilidade = "Docker indisponível para os testes de integração com SQL Server.";
            return;
        }

        await using (var contexto = CriarContexto())
        {
            await contexto.Database.EnsureCreatedAsync();
        }

        await using var servicos = CriarServicos();
        using var escopo = servicos.CreateScope();
        var roleManager = escopo.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var nomeRole in new[] { RoleNames.Admin, RoleNames.Funcionario })
        {
            if (!await roleManager.RoleExistsAsync(nomeRole))
                await roleManager.CreateAsync(new ApplicationRole { Name = nomeRole });
        }
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }

    private void GarantirDisponivel()
    {
        if (MotivoIndisponibilidade is not null)
            throw new InvalidOperationException(MotivoIndisponibilidade);
    }

    private static bool DockerIndisponivel(Exception ex)
        => ex is ArgumentException { ParamName: "DockerEndpointAuthConfig" }
           || ex.Message.Contains("Docker is either not running or misconfigured", StringComparison.OrdinalIgnoreCase);
}

[CollectionDefinition(nameof(SqlServerCollection))]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerFixture>;
