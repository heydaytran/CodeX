namespace Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;

    public PagedResult() { }

    public PagedResult(IEnumerable<T> items, int totalRecords, int currentPage, int pageSize)
    {
        Items = items;
        TotalRecords = totalRecords;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
    }

    public static PagedResult<T> Create(IEnumerable<T> items, int totalRecords, int currentPage, int pageSize)
    {
        return new PagedResult<T>(items, totalRecords, currentPage, pageSize);
    }
}