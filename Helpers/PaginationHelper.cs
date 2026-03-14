namespace TechnicalTest.Api.Helpers;

public class PaginationHelper
{
    public static PagedResult<T> CreatePagedResult<T>(
        IEnumerable<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            HasPrevious = pageNumber > 1,
            HasNext = pageNumber < totalPages
        };
    }

    public static int ValidatePageSize(int pageSize, int maxPageSize = 100, int defaultPageSize = 20)
    {
        if (pageSize <= 0)
            return defaultPageSize;

        return Math.Min(pageSize, maxPageSize);
    }

    public static int ValidatePageNumber(int pageNumber)
    {
        return Math.Max(pageNumber, 1);
    }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
