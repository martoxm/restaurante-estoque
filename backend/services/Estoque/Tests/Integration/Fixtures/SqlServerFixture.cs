using RestauranteEstoque.Core.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Application;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Infrastructure;
using RestauranteEstoque.Infrastructure.Persistence;
using RestauranteEstoque.Tests.Common;
using Testcontainers.MsSql;

namespace RestauranteEstoque.Tests.Integration.Fixtures;

public sealed class SqlServerFixture : IAsyncLifetime
{
    private MsSqlContainer? _container;

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

        return new AppDbContext(options, usuarioAtualService ?? new FakeUsuarioAtualService { RestauranteId = DadosTeste.RestauranteId });
    }

    public ServiceProvider CriarServicos(IUsuarioAtualService? usuarioAtualService = null)
    {
        GarantirDisponivel();

        var configuracao = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString
                                                                            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(configuracao);
        services.AddSingleton(usuarioAtualService ?? new FakeUsuarioAtualService { RestauranteId = DadosTeste.RestauranteId });

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
