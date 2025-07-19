namespace Domain.Primitives;

/// <summary>
/// Represents the abstract entity primitive.
/// </summary>
/// <typeparam name="TEntityId">The entity identifier type.</typeparam>
public abstract class Entity<TEntityId> : IEquatable<Entity<TEntityId>>, IEntity
    where TEntityId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(TEntityId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id), "The entity identifier is required.");
    }

    /// <remarks>
    /// Required for deserialization.
    /// </remarks>
    protected Entity() { }

    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    public required TEntityId Id { get; init; } 
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? DeletedAt { get; set; }

    public static bool operator ==(Entity<TEntityId>? a, Entity<TEntityId>? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TEntityId>? a, Entity<TEntityId>? b) => !(a == b);

    /// <inheritdoc />
    public virtual bool Equals(Entity<TEntityId>? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Entity<TEntityId> entity && Equals(entity);
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <inheritdoc />
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <inheritdoc />
    public IReadOnlyList<IDomainEvent> GetDomainEvents() => [.. _domainEvents];

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}

