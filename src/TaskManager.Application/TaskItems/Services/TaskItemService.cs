using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.TaskItems.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.TaskItems.Services;

public sealed class TaskItemService(IApplicationDbContext db, IDateTimeProvider clock) : ITaskItemService
{
    public async Task<Result<TaskItemResponse>> CreateAsync(TaskItemCreateRequest request, CancellationToken cancellationToken)
    {
        var taskItem = new TaskItem
        {
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

    public Task<Result<TaskItemResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = db.TaskItems.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(taskItem is null
            ? Result<TaskItemResponse>.Failure(Error.NotFound("Task item not found."))
            : Result<TaskItemResponse>.Success(Map(taskItem)));
    }

    public async Task<Result<TaskItemResponse>> UpdateAsync(Guid id, TaskItemUpdateRequest request, CancellationToken cancellationToken)
    {
        var taskItem = db.TaskItems.FirstOrDefault(x => x.Id == id);
        if (taskItem is null) return Result<TaskItemResponse>.Failure(Error.NotFound("Task item not found."));

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
        var taskItem = db.TaskItems.FirstOrDefault(x => x.Id == id);
        if (taskItem is null) return Result<bool>.Failure(Error.NotFound("Task item not found."));

        taskItem.DeletedAtUtc = clock.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<TaskItemResponse>> MoveAsync(Guid id, TaskItemMoveRequest request, CancellationToken cancellationToken)
    {
        var taskItem = db.TaskItems.FirstOrDefault(x => x.Id == id);
        if (taskItem is null) return Result<TaskItemResponse>.Failure(Error.NotFound("Task item not found."));

        taskItem.BoardColumnId = request.BoardColumnId;
        taskItem.Position = request.Position;
        await db.SaveChangesAsync(cancellationToken);
        return Result<TaskItemResponse>.Success(Map(taskItem));
    }

    public Task<Result<PagedResult<TaskItemResponse>>> GetByColumnAsync(Guid boardColumnId, Guid? parentTaskItemId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.TaskItems
            .Where(x => x.BoardColumnId == boardColumnId && x.ParentTaskItemId == parentTaskItemId)
            .OrderBy(x => x.Position)
            .Select(x => new TaskItemResponse(x.Id, x.BoardColumnId, x.ParentTaskItemId, x.Title, x.Description, x.Position, x.DueDateUtc, x.AssigneeId));

        return Task.FromResult(Result<PagedResult<TaskItemResponse>>.Success(PagedResult<TaskItemResponse>.Create(query, page, pageSize)));
    }

    private static TaskItemResponse Map(TaskItem taskItem) =>
        new(taskItem.Id, taskItem.BoardColumnId, taskItem.ParentTaskItemId, taskItem.Title, taskItem.Description, taskItem.Position, taskItem.DueDateUtc, taskItem.AssigneeId);
}
