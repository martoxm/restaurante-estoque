using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Mensageria;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Vendas.Application.Abstractions.Repositories;
using RestauranteEstoque.Vendas.Application.Abstractions.Services;
using RestauranteEstoque.Vendas.Infrastructure.Persistence;
using RestauranteEstoque.Vendas.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Vendas.Infrastructure.Time;

namespace RestauranteEstoque.Vendas.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IOutboxMensagemRepository, OutboxMensagemRepository>();
        services.AddScoped<IMensagemProcessadaRepository, MensagemProcessadaRepository>();
        services.AddScoped<IProdutoCatalogoRepository, ProdutoCatalogoRepository>();

        services.AddMensageriaRabbitMq(configuration);

        services.AddHttpContextAccessor();
        services.AddScoped<UsuarioAtualService>();
        services.AddScoped<IUsuarioAtualService>(provider => provider.GetRequiredService<UsuarioAtualService>());
        services.AddScoped<IDefinidorRestauranteAtual>(provider => provider.GetRequiredService<UsuarioAtualService>());

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRelogio, Relogio>();

        return services;
    }
}
