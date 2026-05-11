namespace TaskManager.Application.Common;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public static async Task<PagedResult<T>> CreateAsync(
        IQueryable<T> query,
        int page,
        int pageSize,
        Func<IQueryable<T>, CancellationToken, Task<int>> countAsync,
        Func<IQueryable<T>, CancellationToken, Task<List<T>>> toListAsync,
        CancellationToken cancellationToken)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
        var total = await countAsync(query, cancellationToken);
        var items = await toListAsync(query.Skip((page - 1) * pageSize).Take(pageSize), cancellationToken);
        return new PagedResult<T>(items, page, pageSize, total);
    }
}
