using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Repositories;
using RestauranteEstoque.Pagamentos.Application.Abstractions.Services;
using RestauranteEstoque.Pagamentos.Infrastructure.Gateways;
using RestauranteEstoque.Pagamentos.Infrastructure.Persistence;
using RestauranteEstoque.Pagamentos.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Pagamentos.Infrastructure.Time;
using Stripe;

namespace RestauranteEstoque.Pagamentos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IPagamentoRepository, PagamentoRepository>();
        services.AddScoped<IOutboxMensagemRepository, OutboxMensagemRepository>();
        services.AddMensageriaRabbitMq(configuration);

        var gatewayFalso = configuration.GetSection("GatewayFalso").Get<GatewayFalsoSettings>() ?? new GatewayFalsoSettings();
        services.AddSingleton(gatewayFalso);
        var stripe = configuration.GetSection("Stripe").Get<StripeSettings>() ?? new StripeSettings();
        services.AddSingleton(stripe);

        var provedor = configuration["Gateway:Provedor"] ?? "Falso";
        if (provedor.Equals("Stripe", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(stripe.ChaveSecreta))
                throw new InvalidOperationException("Gateway:Provedor = Stripe, mas Stripe:ChaveSecreta está vazia (configure no user-secrets).");

            services.AddSingleton(new StripeClient(stripe.ChaveSecreta));
            services.AddSingleton<IGatewayPagamento, GatewayStripe>();
        }
        else
        {
            services.AddSingleton<IGatewayPagamento, GatewayPagamentoFalso>();

            services.AddSingleton(Channel.CreateUnbounded<CobrancaFalsa>());
            services.AddHttpClient(SimuladorWebhookGatewayFalso.NomeHttpClient);
            services.AddHostedService<SimuladorWebhookGatewayFalso>();
        }

        services.AddHttpContextAccessor();
        services.AddScoped<UsuarioAtualService>();
        services.AddScoped<IUsuarioAtualService>(provider => provider.GetRequiredService<UsuarioAtualService>());
        services.AddScoped<IDefinidorRestauranteAtual>(provider => provider.GetRequiredService<UsuarioAtualService>());

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRelogio, Relogio>();

        return services;
    }
}
