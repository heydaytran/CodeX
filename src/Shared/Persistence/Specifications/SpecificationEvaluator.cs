using Domain.Primitives;
using Domain.Specifications;

namespace Persistence.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> Apply<TEntity>(this IQueryable<TEntity> entities, Specification<TEntity> specification)
        where TEntity : class, IEntity
    {
        if (specification.Predicate is not null)
        {
            entities = entities.Where(specification.Predicate);
        }

        entities = specification.IncludeExpressions
            .Aggregate(entities, (current, expression) => current.Include(expression));

        if (specification.OrderBy is not null)
        {
            entities = entities.OrderBy(specification.OrderBy);
        }

        if (specification.OrderByDescending is not null)
        {
            entities = entities.OrderByDescending(specification.OrderByDescending);
        }

        return entities;
    }
}
