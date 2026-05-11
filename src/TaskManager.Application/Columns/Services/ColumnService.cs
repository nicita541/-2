using TaskManager.Application.Abstractions;
using TaskManager.Application.Columns.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Columns.Services;

public sealed class ColumnService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : IColumnService
{
    public async Task<Result<ColumnResponse>> CreateAsync(ColumnCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditBoardAsync(request.BoardId, currentUser.UserId, cancellationToken))
        {
            return Result<ColumnResponse>.Failure(Error.Forbidden("You cannot edit this board."));
        }

        var column = new BoardColumn { BoardId = request.BoardId, Name = request.Name, Position = request.Position };
        db.Add(column);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ColumnResponse>.Success(new ColumnResponse(column.Id, column.BoardId, column.Name, column.Position));
    }

    public async Task<Result<PagedResult<ColumnResponse>>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessBoardAsync(boardId, currentUser.UserId, cancellationToken))
        {
            return Result<PagedResult<ColumnResponse>>.Failure(Error.Forbidden("You cannot access this board."));
        }

        var query = db.BoardColumns.Where(x => x.BoardId == boardId).OrderBy(x => x.Position).Select(x => new ColumnResponse(x.Id, x.BoardId, x.Name, x.Position));
        var result = await PagedResult<ColumnResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<ColumnResponse>>.Success(result);
    }
}
