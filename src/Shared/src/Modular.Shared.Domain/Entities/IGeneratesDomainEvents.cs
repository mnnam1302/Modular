using Modular.Shared.Domain.Events;

namespace Modular.Shared.Domain.Entities;

public interface IGeneratesDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();
}
