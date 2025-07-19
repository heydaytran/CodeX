using Domain.Primitives;

namespace Domain.Specifications;

public abstract class Specification<TEntity>(Expression<Func<TEntity, bool>> predicate)
    where TEntity : IEntity
{
    public Expression<Func<TEntity, bool>> Predicate { get; } = predicate;

    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

    public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; private set; } = [];

    public virtual ErrorOr<TEntity> NotFoundError => Error.NotFound();

    protected void AddOrderBy(Expression<Func<TEntity, object>> expression) =>
        OrderBy = expression;

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> expression) =>
        OrderByDescending = expression;

    protected void AddInclude(Expression<Func<TEntity, object>> expression) =>
        IncludeExpressions.Add(expression);
}