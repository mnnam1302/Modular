namespace Modular.Shared.Domain.Entities.Auditing;

/// <inheritdoc cref="ICreationAuditedObject" />
public abstract class CreationAuditedAggregateRoot<TKey> : AggregateRoot<TKey>, ICreationAuditedObject
{
    public string? CreatorId { get; private set; }

    public DateTimeOffset CreationTime { get; private set; }

    protected CreationAuditedAggregateRoot()
    {
    }

    protected CreationAuditedAggregateRoot(TKey id) : base(id)
    {
    }

    public void SetCreationAudit(string? creatorId, DateTimeOffset creationTime)
    {
        CreatorId = creatorId;
        CreationTime = creationTime;
    }
}
