using TaskManager.Application.Comments.Dtos;
using TaskManager.Application.Common;

namespace TaskManager.Application.Comments.Services;

public interface ICommentService
{
    Task<Result<CommentResponse>> CreateAsync(CommentCreateRequest request, CancellationToken cancellationToken);
    Task<Result<PagedResult<CommentResponse>>> GetByTaskItemAsync(Guid taskItemId, int page, int pageSize, CancellationToken cancellationToken);
}
