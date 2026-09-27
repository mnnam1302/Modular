namespace Modular.Shared.Domain.Entities.Auditing;

public interface ICreationAuditedObject
{
    string? CreatorId { get; }
    DateTimeOffset CreationTime { get; }
}
