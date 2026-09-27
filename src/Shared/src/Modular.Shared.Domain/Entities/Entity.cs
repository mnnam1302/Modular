namespace Modular.Shared.Domain.Entities;

/// <summary>
/// Non-generic marker for any domain entity, regardless of its key type. Lets
/// infrastructure (EF Core model builders, generic repositories, assembly scanners) find
/// "every entity type" without committing to a specific <c>TKey</c>.
/// </summary>
public interface IEntity
{
}

/// <summary>
/// Defines an entity with a single primary key with "Id" property.
/// </summary>
/// <typeparam name="TKey">Type of the primary key of the entity</typeparam>
public interface IEntity<out TKey> : IEntity
{
    TKey Id { get; }
}

/// <inheritdoc cref="IEntity{TKey}" />
/// <remarks>
/// Implements <see cref="IEquatable{T}"/> on an abstract, non-sealed base by design: it
/// is the standard shape for a DDD entity base class, and every derived aggregate is
/// expected to inherit identity-based equality as-is rather than redefine it.
/// </remarks>
#pragma warning disable S4035 // Non-sealed IEquatable<T> is intentional: see remarks above.
public abstract class Entity<TKey> : IEntity<TKey>, IEquatable<Entity<TKey>>
{
    public virtual TKey Id { get; protected set; } = default!;

    protected Entity()
    {
    }

    protected Entity(TKey id) => Id = id;

    /// <summary>
    /// An entity is "transient" while it has not yet been assigned a real identity
    /// (e.g. a newly constructed aggregate that hasn't been persisted, so <see cref="Id"/>
    /// still holds <c>default(TKey)</c>). Transient entities are excluded from
    /// identity-based equality below: two unsaved entities that happen to share the same
    /// default key are not the same conceptual entity, so they must not compare equal.
    /// </summary>
    public virtual bool IsTransient() => EqualityComparer<TKey>.Default.Equals(Id, default!);

    public override string ToString() => $"[ENTITY: {GetType().Name}] Id = {Id}";

    public bool Equals(Entity<TKey>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (IsTransient() || other.IsTransient())
        {
            return false;
        }

        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity<TKey>);

    // Combine with the runtime type (mirroring the GetType() check in Equals) so that,
    // e.g., a Customer with Id = 1 and an Order with Id = 1 don't collide. Uses
    // EqualityComparer<TKey>.Default to stay null-safe when TKey is a reference type.
    public override int GetHashCode() =>
        HashCode.Combine(GetType(), EqualityComparer<TKey>.Default.GetHashCode(Id!));

    public static bool operator ==(Entity<TKey>? a, Entity<TKey>? b) => Equals(a, b);
    public static bool operator !=(Entity<TKey>? a, Entity<TKey>? b) => !Equals(a, b);
}
#pragma warning restore S4035
