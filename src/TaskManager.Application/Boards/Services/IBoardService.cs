using TaskManager.Application.Boards.Dtos;
using TaskManager.Application.Common;

namespace TaskManager.Application.Boards.Services;

public interface IBoardService
{
    Task<Result<BoardResponse>> CreateAsync(BoardCreateRequest request, CancellationToken cancellationToken);
    Task<Result<BoardResponse>> GetAsync(Guid boardId, CancellationToken cancellationToken);
    Task<Result<BoardResponse>> UpdateAsync(Guid boardId, BoardUpdateRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid boardId, CancellationToken cancellationToken);
    Task<Result<PagedResult<BoardResponse>>> GetByProjectAsync(Guid projectId, int page, int pageSize, CancellationToken cancellationToken);
}
