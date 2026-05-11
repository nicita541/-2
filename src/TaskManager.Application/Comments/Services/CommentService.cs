using TaskManager.Application.Abstractions;
using TaskManager.Application.Comments.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Comments.Services;

public sealed class CommentService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : ICommentService
{
    public async Task<Result<CommentResponse>> CreateAsync(CommentCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessTaskItemAsync(request.TaskItemId, currentUser.UserId, cancellationToken))
        {
            return Result<CommentResponse>.Failure(Error.Forbidden("You cannot comment on this task item."));
        }

        var comment = new Comment { TaskItemId = request.TaskItemId, AuthorId = currentUser.UserId, Body = request.Body };
        db.Add(comment);
        await db.SaveChangesAsync(cancellationToken);
        return Result<CommentResponse>.Success(new CommentResponse(comment.Id, comment.TaskItemId, comment.AuthorId, comment.Body, comment.CreatedAtUtc));
    }

    public async Task<Result<PagedResult<CommentResponse>>> GetByTaskItemAsync(Guid taskItemId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessTaskItemAsync(taskItemId, currentUser.UserId, cancellationToken))
        {
            return Result<PagedResult<CommentResponse>>.Failure(Error.Forbidden("You cannot access this task item."));
        }

        var query = db.Comments.Where(x => x.TaskItemId == taskItemId).OrderByDescending(x => x.CreatedAtUtc).Select(x => new CommentResponse(x.Id, x.TaskItemId, x.AuthorId, x.Body, x.CreatedAtUtc));
        var result = await PagedResult<CommentResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<CommentResponse>>.Success(result);
    }
}
