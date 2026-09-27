namespace Modular.Shared.Domain.Entities.Auditing;

/// <inheritdoc cref="IAuditedObject" />
public abstract class AuditedAggregateRoot<TKey> : CreationAuditedAggregateRoot<TKey>, IAuditedObject
{
    public string? LastModifierId { get; private set; }

    public DateTimeOffset? LastModificationTime { get; private set; }

    protected AuditedAggregateRoot()
    {
    }

    protected AuditedAggregateRoot(TKey id) : base(id)
    {
    }

    public void SetModificationAudit(string? modifierId, DateTimeOffset modificationTime)
    {
        LastModifierId = modifierId;
        LastModificationTime = modificationTime;
    }
}
