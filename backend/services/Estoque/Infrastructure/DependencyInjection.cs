using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Infrastructure.Integracoes.Identidade;
using RestauranteEstoque.Core.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Application.Abstractions.Repositories;
using RestauranteEstoque.Application.Abstractions.Services;
using RestauranteEstoque.Infrastructure.EnvioEmail;
using RestauranteEstoque.Infrastructure.Persistence;
using RestauranteEstoque.Infrastructure.Persistence.Interceptors;
using RestauranteEstoque.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Infrastructure.Time;

namespace RestauranteEstoque.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<PublicarEventosDominioInterceptor>();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            options.UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .AddInterceptors(serviceProvider.GetRequiredService<PublicarEventosDominioInterceptor>()));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<IMovimentacaoEstoqueRepository, MovimentacaoEstoqueRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<UsuarioAtualService>();
        services.AddScoped<IUsuarioAtualService>(provider => provider.GetRequiredService<UsuarioAtualService>());
        services.AddScoped<IDefinidorRestauranteAtual>(provider => provider.GetRequiredService<UsuarioAtualService>());

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRelogio, Relogio>();

        services.AddScoped<IOutboxMensagemRepository, OutboxMensagemRepository>();
        services.AddScoped<IMensagemProcessadaRepository, MensagemProcessadaRepository>();
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.AddScoped<IEmailService, SmtpEmailService>();

        services.AddMensageriaRabbitMq(configuration);

        services.Configure<IdentidadeSettings>(configuration.GetSection(IdentidadeSettings.SectionName));
        services.AddHttpClient<IIdentidadeClient, IdentidadeClient>(httpClient =>
            {
                var url = configuration[$"{IdentidadeSettings.SectionName}:Url"]
                    ?? throw new InvalidOperationException("A configuração 'ApiIdentidade:Url' não foi informada.");
                httpClient.BaseAddress = new Uri(url.TrimEnd('/') + "/");
                httpClient.DefaultRequestHeaders.Add(ApiKeyExtensions.NomeCabecalho, configuration[ApiKeyExtensions.ChaveConfiguracao] ?? string.Empty);
            })
            .AddStandardResilienceHandler();

        return services;
    }
}
