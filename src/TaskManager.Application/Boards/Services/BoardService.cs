using TaskManager.Application.Abstractions;
using TaskManager.Application.Boards.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Boards.Services;

public sealed class BoardService(IApplicationDbContext db) : IBoardService
{
    public async Task<Result<BoardResponse>> CreateAsync(BoardCreateRequest request, CancellationToken cancellationToken)
    {
        var board = new Board { ProjectId = request.ProjectId, Name = request.Name };
        db.Add(board);
        await db.SaveChangesAsync(cancellationToken);
        return Result<BoardResponse>.Success(new BoardResponse(board.Id, board.ProjectId, board.Name));
    }

    public Task<Result<PagedResult<BoardResponse>>> GetByProjectAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.Boards.Where(x => x.ProjectId == projectId).Select(x => new BoardResponse(x.Id, x.ProjectId, x.Name));
        return Task.FromResult(Result<PagedResult<BoardResponse>>.Success(PagedResult<BoardResponse>.Create(query, page, pageSize)));
    }
}
