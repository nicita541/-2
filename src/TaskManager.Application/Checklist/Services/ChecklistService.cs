using TaskManager.Application.Abstractions;
using TaskManager.Application.Checklist.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Checklist.Services;

public sealed class ChecklistService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : IChecklistService
{
    public async Task<Result<ChecklistItemResponse>> CreateAsync(ChecklistItemCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditTaskItemAsync(request.TaskItemId, currentUser.UserId, cancellationToken))
        {
            return Result<ChecklistItemResponse>.Failure(Error.Forbidden("You cannot edit checklist on this task item."));
        }

        var item = new ChecklistItem { TaskItemId = request.TaskItemId, Text = request.Text, Position = request.Position };
        db.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ChecklistItemResponse>.Success(new ChecklistItemResponse(item.Id, item.TaskItemId, item.Text, item.IsCompleted, item.Position));
    }

    public async Task<Result<ChecklistItemResponse>> UpdateAsync(Guid id, ChecklistItemUpdateRequest request, CancellationToken cancellationToken)
    {
        var item = await db.FirstOrDefaultAsync(db.ChecklistItems.Where(x => x.Id == id), cancellationToken);
        if (item is null) return Result<ChecklistItemResponse>.Failure(Error.NotFound("Checklist item not found."));

        if (!await permissions.CanAccessChecklistItemAsync(id, currentUser.UserId, cancellationToken))
        {
            return Result<ChecklistItemResponse>.Failure(Error.Forbidden("You cannot edit this checklist item."));
        }

        item.Text = request.Text;
        item.IsCompleted = request.IsCompleted;
        item.Position = request.Position;
        await db.SaveChangesAsync(cancellationToken);
        return Result<ChecklistItemResponse>.Success(new ChecklistItemResponse(item.Id, item.TaskItemId, item.Text, item.IsCompleted, item.Position));
    }
}
