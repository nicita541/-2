using TaskManager.Application.Abstractions;
using TaskManager.Application.Comments.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Comments.Services;

public sealed class CommentService(IApplicationDbContext db, ICurrentUserService currentUser) : ICommentService
{
    public async Task<Result<CommentResponse>> CreateAsync(CommentCreateRequest request, CancellationToken cancellationToken)
    {
        var comment = new Comment { TaskItemId = request.TaskItemId, AuthorId = currentUser.UserId, Body = request.Body };
        db.Add(comment);
        await db.SaveChangesAsync(cancellationToken);
        return Result<CommentResponse>.Success(new CommentResponse(comment.Id, comment.TaskItemId, comment.AuthorId, comment.Body, comment.CreatedAtUtc));
    }

    public Task<Result<PagedResult<CommentResponse>>> GetByTaskItemAsync(Guid taskItemId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.Comments.Where(x => x.TaskItemId == taskItemId).OrderByDescending(x => x.CreatedAtUtc).Select(x => new CommentResponse(x.Id, x.TaskItemId, x.AuthorId, x.Body, x.CreatedAtUtc));
        return Task.FromResult(Result<PagedResult<CommentResponse>>.Success(PagedResult<CommentResponse>.Create(query, page, pageSize)));
    }
}
