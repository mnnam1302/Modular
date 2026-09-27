namespace Modular.Shared.Domain.Entities;

/// <summary>
/// Non-generic marker for any aggregate root, regardless of its key type. Lets
/// infrastructure find "every aggregate root type" (e.g. to wire up repositories or
/// outbox dispatch by convention) without committing to a specific <c>TKey</c>.
/// </summary>
public interface IAggregateRoot : IEntity
{
}

/// <summary>
/// Defines an aggregate root with a single primary key with "Id" property.
/// An aggregate root is the only entity within its aggregate that outside code is
/// allowed to hold a reference to or load directly - everything else in the aggregate
/// is reached, and its invariants enforced, through it.
/// </summary>
/// <typeparam name="TKey">Type of the primary key of the entity</typeparam>
public interface IAggregateRoot<out TKey> : IAggregateRoot, IEntity<TKey>
{
}
