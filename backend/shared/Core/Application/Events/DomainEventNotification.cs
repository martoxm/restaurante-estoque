using MediatR;
using RestauranteEstoque.Core.Domain;

namespace RestauranteEstoque.Core.Application.Events;

public sealed record DomainEventNotification<TEvento>(TEvento Evento) : INotification
    where TEvento : IDomainEvent;
