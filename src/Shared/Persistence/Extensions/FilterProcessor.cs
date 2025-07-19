using System.Linq.Expressions;
using Application.Common;

namespace Persistence.Extensions;

public static class FilterProcessor
{
    public static IQueryable<T> ApplySorting<T>(QueryParameters<T> queryParameters, IQueryable<T> query)
    {
        foreach (var sortField in queryParameters.SortBy)
        {
            bool descending = sortField.StartsWith("-");
            string fieldName = descending ? sortField.Substring(1) : sortField;

            var param = Expression.Parameter(typeof(T), "x");
            var property = GetNestedPropertyExpression(param, typeof(T), fieldName);

            if (property == null)
            {
                continue;
            }

            var lambda = Expression.Lambda(property, param);
            string method = descending ? "OrderByDescending" : "OrderBy";
            var methodCall = Expression.Call(
                typeof(Queryable),
                method,
                new Type[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda)
            );

            query = query.Provider.CreateQuery<T>(methodCall);
        }

        return query;
    }

    public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, QueryFilter filter)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var property = GetNestedPropertyExpression(param, typeof(T), filter.PropertyName);

        if (property == null)
        {
            return query;
        }

        object? convertedValue;

        if (property.Type == typeof(bool) && filter.Value is not null)
        {
            convertedValue = bool.TryParse(filter.Value.ToString(), out var boolResult) 
                ? boolResult 
                : throw new ArgumentException($"Invalid boolean value: {filter.Value}");
        }
        else if (property.Type == typeof(string) && filter.Value is not null)
        {
            convertedValue = filter.Value.ToString();
        }
        else
        {
            convertedValue = Convert.ChangeType(filter.Value, property.Type);
        }

        var value = Expression.Constant(convertedValue, property.Type);
        bool isString = property.Type == typeof(string);
        bool isBoolean = property.Type == typeof(bool);

        Expression? predicate = filter.Operator switch
        {
            "="  => isString 
                ? Expression.Call(property, typeof(string).GetMethod("Equals", new[] { typeof(string) })!, value) 
                : Expression.Equal(property, value),
            
            "!=" => isString 
                ? Expression.Not(Expression.Call(property, typeof(string).GetMethod("Equals", new[] { typeof(string) })!, value)) 
                : Expression.NotEqual(property, value),

            ">"  => !isBoolean ? Expression.GreaterThan(property, value) : throw new NotSupportedException("Cannot use '>' on boolean"),
            "<"  => !isBoolean ? Expression.LessThan(property, value) : throw new NotSupportedException("Cannot use '<' on boolean"),
            ">=" => !isBoolean ? Expression.GreaterThanOrEqual(property, value) : throw new NotSupportedException("Cannot use '>=' on boolean"),
            "<=" => !isBoolean ? Expression.LessThanOrEqual(property, value) : throw new NotSupportedException("Cannot use '<=' on boolean"),
            
            "@=" => isString 
                ? Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, value) 
                : throw new NotSupportedException("Contains is only for strings"),
            
            _    => throw new NotSupportedException($"Operator '{filter.Operator}' not supported")
        };

        var lambda = Expression.Lambda<Func<T, bool>>(predicate, param);
        return query.Where(lambda);
    }

    
    private static MemberExpression? GetNestedPropertyExpression(Expression param, Type type, string propertyPath)
    {
        string[] properties = propertyPath.Split('.');
        Expression? propertyExpression = param;

        foreach (var propertyName in properties)
        {
            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return null;
            }

            propertyExpression = Expression.Property(propertyExpression, propertyInfo);
            type = propertyInfo.PropertyType;
        }

        return propertyExpression as MemberExpression;
    }
}