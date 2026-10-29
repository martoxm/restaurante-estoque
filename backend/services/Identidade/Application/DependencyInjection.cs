using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RestauranteEstoque.Core.Application.Behaviors;

namespace RestauranteEstoque.Identidade.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<UseCases.Auth.EmissorSessao>();

        return services;
    }
}
