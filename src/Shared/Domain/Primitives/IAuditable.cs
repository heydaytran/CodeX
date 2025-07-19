namespace Domain.Primitives;

public interface IBaseAuditable;

public interface IAuditable : IAuditableCreatedAt, IAuditableLastActivityAt;

public interface IAuditableCreatedAt : IBaseAuditable
{
    DateTime CreatedAt { get; }
}

public interface IAuditableLastActivityAt : IBaseAuditable
{
    DateTime LastActivityAt { get; }
}