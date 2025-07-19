namespace Domain.Primitives;

public interface IHasOptionalUserId
{
    public Guid? UserId { get; }
}
