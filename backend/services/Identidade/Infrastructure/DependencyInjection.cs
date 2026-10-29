using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Core.Application.Abstractions;
using RestauranteEstoque.Core.Seguranca;
using RestauranteEstoque.Identidade.Application.Abstractions.Repositories;
using RestauranteEstoque.Identidade.Application.Abstractions.Services;
using RestauranteEstoque.Identidade.Infrastructure.Identity;
using RestauranteEstoque.Identidade.Infrastructure.Persistence;
using RestauranteEstoque.Identidade.Infrastructure.Persistence.Repositories;
using RestauranteEstoque.Identidade.Infrastructure.Security;
using RestauranteEstoque.Identidade.Infrastructure.Time;

namespace RestauranteEstoque.Identidade.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString,
                    sqlServer => sqlServer.MigrationsHistoryTable("__EFMigrationsHistory", "identidade"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<ApplicationRole>()
            .AddErrorDescriber<IdentityErrorDescriberPtBr>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IRestauranteRepository, RestauranteRepository>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddSingleton<IGeradorRefreshToken, GeradorRefreshToken>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IRelogio, Relogio>();

        return services;
    }
}
