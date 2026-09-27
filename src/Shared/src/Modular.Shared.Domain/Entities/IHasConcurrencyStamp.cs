namespace Modular.Shared.Domain.Entities;

/// <summary>
/// Opt-in marker for entities that need application-level optimistic concurrency
/// control, e.g. mapped by infrastructure to a token that changes on every update and
/// is checked on save. Not applied to <see cref="AggregateRoot{TKey}"/> by default -
/// add it only to the specific aggregates that need it, since many are equally well
/// served by a database-native row version instead.
/// </summary>
public interface IHasConcurrencyStamp
{
    string ConcurrencyStamp { get; }
}
