namespace Modular.Shared.Domain.Entities.Auditing;

/// <inheritdoc cref="IFullAuditedObject" />
public abstract class FullAuditedAggregateRoot<TKey> : AuditedAggregateRoot<TKey>, IFullAuditedObject
{
    public bool IsDeleted { get; private set; }

    public string? DeleterId { get; private set; }

    public DateTimeOffset? DeletionTime { get; private set; }

    protected FullAuditedAggregateRoot()
    {
    }

    protected FullAuditedAggregateRoot(TKey id) : base(id)
    {
    }

    /// <summary>
    /// Soft-deletes this entity. Idempotent: calling it again on an already-deleted
    /// entity has no effect. Override to also <c>RaiseDomainEvent</c> alongside the
    /// state change if other parts of the system need to react to the deletion.
    /// </summary>
    public virtual void Delete(string? deleterId, DateTimeOffset deletionTime)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeleterId = deleterId;
        DeletionTime = deletionTime;
    }

    /// <summary>
    /// Reverses a soft-delete. Idempotent: calling it on an entity that isn't deleted
    /// has no effect.
    /// </summary>
    public virtual void Restore()
    {
        if (!IsDeleted)
        {
            return;
        }

        IsDeleted = false;
        DeleterId = null;
        DeletionTime = null;
    }
}
