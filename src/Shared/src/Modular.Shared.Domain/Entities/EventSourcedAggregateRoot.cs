using Modular.Shared.Domain.Events;

namespace Modular.Shared.Domain.Entities;

/// <summary>
/// An aggregate root whose current state is derived entirely by folding the sequence of
/// domain events raised against it, rather than being persisted as a current-state row.
/// Derived classes implement <see cref="Apply"/> to fold a single event into the
/// aggregate's in-memory state; <see cref="RaiseEvent"/> is the only supported way for a
/// behavior method to change that state, which keeps "what changed" and "why" identical.
/// </summary>
/// <typeparam name="TKey">Type of the primary key / event-stream identifier.</typeparam>
public abstract class EventSourcedAggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IGeneratesDomainEvents
{
    private static readonly IReadOnlyCollection<IDomainEvent> _emptyEvents = [];

    private List<IDomainEvent>? _uncommittedEvents;

    /// <summary>
    /// Number of events that have been applied to this aggregate so far (from history
    /// and/or newly raised), starting at -1 for a brand-new aggregate. An event store
    /// uses this as the expected stream position when appending, to detect concurrent
    /// writes to the same aggregate (optimistic concurrency).
    /// </summary>
    public long Version { get; private set; } = -1;

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _uncommittedEvents?.AsReadOnly() ?? _emptyEvents;

    protected EventSourcedAggregateRoot()
    {
    }

    protected EventSourcedAggregateRoot(TKey id) : base(id)
    {
    }

    protected abstract void Apply(IDomainEvent domainEvent);

    protected void RaiseEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        Apply(domainEvent);
        Version++;

        _uncommittedEvents ??= [];
        _uncommittedEvents.Add(domainEvent);
    }

    public void LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        ArgumentNullException.ThrowIfNull(history);

        foreach (IDomainEvent domainEvent in history)
        {
            Apply(domainEvent);
            Version++;
        }
    }

    public void ClearDomainEvents() => _uncommittedEvents?.Clear();
}
