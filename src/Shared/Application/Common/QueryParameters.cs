using System.Linq.Expressions;

namespace Application.Common;

public class QueryParameters<T>
{
    public List<QueryFilter> Filters { get; set; } = new();
    public List<string> SortBy { get; set; } = new(); // Supports multiple sorts
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public Expression<Func<T, bool>>? FilterExpression { get; set; }
}