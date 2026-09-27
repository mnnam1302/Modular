namespace Modular.Shared.Domain.Entities.Auditing;

/// <summary>
/// This interface can be implemented to store deletion information (who delete and when deleted).
/// </summary>
public interface IDeletionAuditedObject
{
    string? DeleterId { get; }
    DateTimeOffset? DeletionTime { get; }
}
