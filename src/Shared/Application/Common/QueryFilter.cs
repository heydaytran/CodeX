namespace Application.Common;

public class QueryFilter
{
    public string PropertyName { get; set; } = string.Empty;
    public string Operator { get; set; } = "="; // e.g., =, >, <, >=, <=, @= (contains)
    public object? Value { get; set; }
}