using Modular.Shared.Domain.Entities;

namespace Modular.Shared.Domain.Events;

/// <summary>
/// Something that happened in the domain that other parts of the system may need to
/// react to. Domain events are immutable facts about the past, raised by an
/// <see cref="AggregateRoot{TKey}"/> (or <see cref="EventSourcedAggregateRoot{TKey}"/>)
/// as a side effect of a behavior method, and dispatched - in-process and/or via an
/// outbox - only after the originating unit of work has been committed.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Unique identifier of this particular occurrence. Used for de-duplication under
    /// at-least-once delivery (e.g. an outbox) and for correlating logs/traces - not a
    /// business identifier.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// UTC point in time at which the event occurred in the domain (i.e. when it was
    /// raised), as distinct from whenever a handler eventually processes it.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}

/// <inheritdoc cref="IDomainEvent" />
/// <remarks>
/// Base class for concrete, named domain events (e.g. <c>OrderPlacedDomainEvent</c>).
/// It is abstract because a bare, unnamed "DomainEvent" carries no meaning by itself -
/// every real event should describe a specific thing that happened.
/// </remarks>
public abstract class DomainEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
