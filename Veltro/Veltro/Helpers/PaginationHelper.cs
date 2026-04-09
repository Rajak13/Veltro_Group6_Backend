namespace Veltro.Helpers;

/// <summary>Pagination metadata returned alongside paged list responses.</summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>Applies skip/take pagination to an IQueryable.</summary>
public static class PaginationHelper
{
    /// <summary>Returns a paged slice of a queryable source.</summary>
    public static async Task<PagedResult<T>> PaginateAsync<T>(
        IQueryable<T> source, int page, int pageSize)
    {
        var totalCount = source.Count();
        var items = source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
