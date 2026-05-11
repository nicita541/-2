using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.TaskItems.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.TaskItems.Services;

public sealed class TaskItemService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions, IDateTimeProvider clock) : ITaskItemService
{
    public async Task<Result<TaskItemResponse>> CreateAsync(TaskItemCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditProjectAsync(request.ProjectId, currentUser.UserId, cancellationToken))
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("You cannot edit this project."));
        }

        var columnProjectId = await GetColumnProjectIdAsync(request.BoardColumnId, cancellationToken);
        if (columnProjectId != request.ProjectId)
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("Column does not belong to the requested project."));
        }

        if (request.ParentTaskItemId.HasValue)
        {
            var parentProjectId = await db.FirstOrDefaultAsync(
                db.TaskItems.Where(x => x.Id == request.ParentTaskItemId.Value).Select(x => x.ProjectId),
                cancellationToken);

            if (parentProjectId != request.ProjectId)
            {
                return Result<TaskItemResponse>.Failure(Error.Forbidden("Parent task item belongs to another project."));
            }
        }

        if (request.AssigneeId.HasValue && !await permissions.IsProjectWorkspaceMemberAsync(request.ProjectId, request.AssigneeId.Value, cancellationToken))
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("Assignee must be a workspace member."));
        }

        var taskItem = new TaskItem
        {
            ProjectId = request.ProjectId,
            BoardColumnId = request.BoardColumnId,
            ParentTaskItemId = request.ParentTaskItemId,
            Title = request.Title,
            Description = request.Description,
            Position = request.Position,
            DueDateUtc = request.DueDateUtc,
            AssigneeId = request.AssigneeId
        };
        db.Add(taskItem);
        await db.SaveChangesAsync(cancellationToken);
        return Result<TaskItemResponse>.Success(Map(taskItem));
    }

    public async Task<Result<TaskItemResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessTaskItemAsync(id, currentUser.UserId, cancellationToken))
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("You cannot access this task item."));
        }

        var taskItem = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == id), cancellationToken);
        return taskItem is null
            ? Result<TaskItemResponse>.Failure(Error.NotFound("Task item not found."))
            : Result<TaskItemResponse>.Success(Map(taskItem));
    }

    public async Task<Result<TaskItemResponse>> UpdateAsync(Guid id, TaskItemUpdateRequest request, CancellationToken cancellationToken)
    {
        var taskItem = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == id), cancellationToken);
        if (taskItem is null) return Result<TaskItemResponse>.Failure(Error.NotFound("Task item not found."));

        if (!await permissions.CanEditTaskItemAsync(id, currentUser.UserId, cancellationToken) ||
            !await permissions.CanEditProjectAsync(request.ProjectId, currentUser.UserId, cancellationToken))
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("You cannot edit this task item."));
        }

        var columnProjectId = await GetColumnProjectIdAsync(request.BoardColumnId, cancellationToken);
        if (columnProjectId != request.ProjectId)
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("Column does not belong to the requested project."));
        }

        if (request.AssigneeId.HasValue && !await permissions.IsProjectWorkspaceMemberAsync(request.ProjectId, request.AssigneeId.Value, cancellationToken))
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("Assignee must be a workspace member."));
        }

        taskItem.ProjectId = request.ProjectId;
        taskItem.Title = request.Title;
        taskItem.Description = request.Description;
        taskItem.Position = request.Position;
        taskItem.DueDateUtc = request.DueDateUtc;
        taskItem.AssigneeId = request.AssigneeId;
        taskItem.BoardColumnId = request.BoardColumnId;
        await db.SaveChangesAsync(cancellationToken);
        return Result<TaskItemResponse>.Success(Map(taskItem));
    }

    public async Task<Result<bool>> SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == id), cancellationToken);
        if (taskItem is null) return Result<bool>.Failure(Error.NotFound("Task item not found."));

        if (!await permissions.CanEditTaskItemAsync(id, currentUser.UserId, cancellationToken))
        {
            return Result<bool>.Failure(Error.Forbidden("You cannot delete this task item."));
        }

        taskItem.DeletedAtUtc = clock.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<TaskItemResponse>> MoveAsync(Guid id, TaskItemMoveRequest request, CancellationToken cancellationToken)
    {
        var taskItem = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == id), cancellationToken);
        if (taskItem is null) return Result<TaskItemResponse>.Failure(Error.NotFound("Task item not found."));

        if (!await permissions.CanEditTaskItemAsync(id, currentUser.UserId, cancellationToken))
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("You cannot move this task item."));
        }

        var columnProjectId = await GetColumnProjectIdAsync(request.BoardColumnId, cancellationToken);
        if (columnProjectId != taskItem.ProjectId)
        {
            return Result<TaskItemResponse>.Failure(Error.Forbidden("Target column belongs to another project."));
        }

        taskItem.BoardColumnId = request.BoardColumnId;
        taskItem.Position = request.Position;
        await db.SaveChangesAsync(cancellationToken);
        return Result<TaskItemResponse>.Success(Map(taskItem));
    }

    public async Task<Result<PagedResult<TaskItemResponse>>> GetByColumnAsync(Guid boardColumnId, Guid? parentTaskItemId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessColumnAsync(boardColumnId, currentUser.UserId, cancellationToken))
        {
            return Result<PagedResult<TaskItemResponse>>.Failure(Error.Forbidden("You cannot access this column."));
        }

        var query = db.TaskItems
            .Where(x => x.BoardColumnId == boardColumnId && x.ParentTaskItemId == parentTaskItemId)
            .OrderBy(x => x.Position)
            .Select(x => new TaskItemResponse(x.Id, x.ProjectId, x.BoardColumnId, x.ParentTaskItemId, x.Title, x.Description, x.Position, x.DueDateUtc, x.AssigneeId));

        var result = await PagedResult<TaskItemResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<TaskItemResponse>>.Success(result);
    }

    private async Task<Guid> GetColumnProjectIdAsync(Guid boardColumnId, CancellationToken cancellationToken) =>
        await db.FirstOrDefaultAsync(db.BoardColumns.Where(x => x.Id == boardColumnId).Select(x => x.Board!.ProjectId), cancellationToken);

    private static TaskItemResponse Map(TaskItem taskItem) =>
        new(taskItem.Id, taskItem.ProjectId, taskItem.BoardColumnId, taskItem.ParentTaskItemId, taskItem.Title, taskItem.Description, taskItem.Position, taskItem.DueDateUtc, taskItem.AssigneeId);
}
