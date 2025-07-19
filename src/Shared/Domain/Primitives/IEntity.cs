namespace Domain.Primitives;

public interface IEntity
{    
    IReadOnlyList<IDomainEvent> GetDomainEvents();

    void ClearDomainEvents();
}
