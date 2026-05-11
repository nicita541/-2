using TaskManager.Application.Abstractions;
using TaskManager.Application.Boards.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Boards.Services;

public sealed class BoardService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : IBoardService
{
    public async Task<Result<BoardResponse>> CreateAsync(BoardCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditProjectAsync(request.ProjectId, currentUser.UserId, cancellationToken))
        {
            return Result<BoardResponse>.Failure(Error.Forbidden("You cannot edit this project."));
        }

        var board = new Board { ProjectId = request.ProjectId, Name = request.Name };
        db.Add(board);
        await db.SaveChangesAsync(cancellationToken);
        return Result<BoardResponse>.Success(new BoardResponse(board.Id, board.ProjectId, board.Name));
    }

    public async Task<Result<PagedResult<BoardResponse>>> GetByProjectAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessProjectAsync(projectId, currentUser.UserId, cancellationToken))
        {
            return Result<PagedResult<BoardResponse>>.Failure(Error.Forbidden("You cannot access this project."));
        }

        var query = db.Boards.Where(x => x.ProjectId == projectId).Select(x => new BoardResponse(x.Id, x.ProjectId, x.Name));
        var result = await PagedResult<BoardResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<BoardResponse>>.Success(result);
    }
}
