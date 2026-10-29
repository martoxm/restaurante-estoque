using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RestauranteEstoque.Core.Application.Events;
using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Infrastructure.Persistence.Interceptors;

public class PublicarEventosDominioInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;

    public PublicarEventosDominioInterceptor(IPublisher publisher) => _publisher = publisher;

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            await PublicarEventosAsync(eventData.Context, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task PublicarEventosAsync(DbContext context, CancellationToken cancellationToken)
    {
        var entidades = context.ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .Where(entidade => entidade.Eventos.Count > 0)
            .ToList();

        var eventos = entidades.SelectMany(entidade => entidade.Eventos).ToList();
        entidades.ForEach(entidade => entidade.LimparEventos());

        foreach (var evento in eventos)
        {
            var tipoNotificacao = typeof(DomainEventNotification<>).MakeGenericType(evento.GetType());
            var notificacao = Activator.CreateInstance(tipoNotificacao, evento)!;

            await _publisher.Publish(notificacao, cancellationToken);
        }
    }
}
