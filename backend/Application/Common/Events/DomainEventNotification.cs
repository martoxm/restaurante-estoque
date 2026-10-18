using MediatR;
using RestauranteEstoque.Domain.Common;

namespace RestauranteEstoque.Application.Common.Events;

public sealed record DomainEventNotification<TEvento>(TEvento Evento) : INotification
    where TEvento : IDomainEvent;
