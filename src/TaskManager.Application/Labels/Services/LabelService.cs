using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Labels.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Labels.Services;

public sealed class LabelService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : ILabelService
{
    public async Task<Result<LabelResponse>> CreateAsync(LabelCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditBoardAsync(request.BoardId, currentUser.UserId, cancellationToken))
        {
            return Result<LabelResponse>.Failure(Error.Forbidden("You cannot edit labels on this board."));
        }

        var label = new Label { BoardId = request.BoardId, Name = request.Name, ColorHex = request.ColorHex };
        db.Add(label);
        await db.SaveChangesAsync(cancellationToken);
        return Result<LabelResponse>.Success(new LabelResponse(label.Id, label.BoardId, label.Name, label.ColorHex));
    }

    public async Task<Result<bool>> AssignAsync(Guid taskItemId, Guid labelId, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditTaskItemAsync(taskItemId, currentUser.UserId, cancellationToken) ||
            !await permissions.CanEditLabelAsync(labelId, currentUser.UserId, cancellationToken))
        {
            return Result<bool>.Failure(Error.Forbidden("You cannot assign this label."));
        }

        var taskProjectId = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == taskItemId).Select(x => x.ProjectId), cancellationToken);
        var labelProjectId = await db.FirstOrDefaultAsync(db.Labels.Where(x => x.Id == labelId).Select(x => x.Board!.ProjectId), cancellationToken);
        if (taskProjectId == Guid.Empty || taskProjectId != labelProjectId)
        {
            return Result<bool>.Failure(Error.Forbidden("Label and task item must belong to the same project."));
        }

        if (await db.AnyAsync(db.TaskItemLabels.Where(x => x.TaskItemId == taskItemId && x.LabelId == labelId), cancellationToken))
        {
            return Result<bool>.Success(true);
        }

        db.Add(new TaskItemLabel { TaskItemId = taskItemId, LabelId = labelId });
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResult<LabelResponse>>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessBoardAsync(boardId, currentUser.UserId, cancellationToken))
        {
            return Result<PagedResult<LabelResponse>>.Failure(Error.Forbidden("You cannot access this board."));
        }

        var query = db.Labels.Where(x => x.BoardId == boardId).Select(x => new LabelResponse(x.Id, x.BoardId, x.Name, x.ColorHex));
        var result = await PagedResult<LabelResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<LabelResponse>>.Success(result);
    }
}
