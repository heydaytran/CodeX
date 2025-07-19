namespace Domain.Primitives;

public interface IPageList
{
    public List<string> Includes { get; init; }
    public List<FilterModel> Filters { get; init; }
    public List<string> Sorts { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public record FilterModel(string FieldName, string Comparision, string FieldValue);
