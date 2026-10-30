using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RestauranteEstoque.Core.Mensageria;

public static class MensageriaExtensions
{
    public static IServiceCollection AddMensageriaRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));
        services.AddSingleton<ConexaoRabbitMq>();
        services.AddSingleton<IPublicadorMensagens, RabbitMqPublicador>();

        return services;
    }
}
