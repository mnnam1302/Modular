namespace Modular.Shared.Domain.Entities.Auditing;

public interface IModificationAuditedObject
{
    string? LastModifierId { get; }
    DateTimeOffset? LastModificationTime { get; }
}
