namespace Domain.Primitives;

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    long Version { get; set; }
}

public interface IAggregate<out T> : IAggregate
{
    T Id { get; }
}