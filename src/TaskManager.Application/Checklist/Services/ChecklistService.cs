using TaskManager.Application.Abstractions;
using TaskManager.Application.Checklist.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Checklist.Services;

public sealed class ChecklistService(IApplicationDbContext db) : IChecklistService
{
    public async Task<Result<ChecklistItemResponse>> CreateAsync(ChecklistItemCreateRequest request, CancellationToken cancellationToken)
    {
        var item = new ChecklistItem { TaskItemId = request.TaskItemId, Text = request.Text, Position = request.Position };
        db.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ChecklistItemResponse>.Success(new ChecklistItemResponse(item.Id, item.TaskItemId, item.Text, item.IsCompleted, item.Position));
    }

    public async Task<Result<ChecklistItemResponse>> UpdateAsync(Guid id, ChecklistItemUpdateRequest request, CancellationToken cancellationToken)
    {
        var item = db.ChecklistItems.FirstOrDefault(x => x.Id == id);
        if (item is null) return Result<ChecklistItemResponse>.Failure(Error.NotFound("Checklist item not found."));

        item.Text = request.Text;
        item.IsCompleted = request.IsCompleted;
        item.Position = request.Position;
        await db.SaveChangesAsync(cancellationToken);
        return Result<ChecklistItemResponse>.Success(new ChecklistItemResponse(item.Id, item.TaskItemId, item.Text, item.IsCompleted, item.Position));
    }
}
