namespace Domain.Primitives;

public record ListResultModel<T>(List<T> Items, long TotalItems, int Page, int PageSize)
    where T : notnull
{
    public static ListResultModel<T> Empty => new(Enumerable.Empty<T>().ToList(), 0, 0, 0);

    public static ListResultModel<T> Create(List<T> items, long totalItems = 0, int page = 0, int pageSize = 20)
    {
        return new ListResultModel<T>(items, totalItems, page, pageSize);
    }
}