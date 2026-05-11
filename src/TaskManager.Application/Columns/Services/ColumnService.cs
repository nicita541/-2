using TaskManager.Application.Abstractions;
using TaskManager.Application.Columns.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Columns.Services;

public sealed class ColumnService(IApplicationDbContext db) : IColumnService
{
    public async Task<Result<ColumnResponse>> CreateAsync(ColumnCreateRequest request, CancellationToken cancellationToken)
    {
        var column = new BoardColumn { BoardId = request.BoardId, Name = request.Name, Position = request.Position };
        db.Add(column);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ColumnResponse>.Success(new ColumnResponse(column.Id, column.BoardId, column.Name, column.Position));
    }

    public Task<Result<PagedResult<ColumnResponse>>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.BoardColumns.Where(x => x.BoardId == boardId).OrderBy(x => x.Position).Select(x => new ColumnResponse(x.Id, x.BoardId, x.Name, x.Position));
        return Task.FromResult(Result<PagedResult<ColumnResponse>>.Success(PagedResult<ColumnResponse>.Create(query, page, pageSize)));
    }
}
