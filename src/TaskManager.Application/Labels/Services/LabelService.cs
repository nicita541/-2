using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Labels.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Labels.Services;

public sealed class LabelService(IApplicationDbContext db) : ILabelService
{
    public async Task<Result<LabelResponse>> CreateAsync(LabelCreateRequest request, CancellationToken cancellationToken)
    {
        var label = new Label { BoardId = request.BoardId, Name = request.Name, ColorHex = request.ColorHex };
        db.Add(label);
        await db.SaveChangesAsync(cancellationToken);
        return Result<LabelResponse>.Success(new LabelResponse(label.Id, label.BoardId, label.Name, label.ColorHex));
    }

    public async Task<Result<bool>> AssignAsync(Guid taskItemId, Guid labelId, CancellationToken cancellationToken)
    {
        if (db.TaskItemLabels.Any(x => x.TaskItemId == taskItemId && x.LabelId == labelId))
        {
            return Result<bool>.Success(true);
        }

        db.Add(new TaskItemLabel { TaskItemId = taskItemId, LabelId = labelId });
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public Task<Result<PagedResult<LabelResponse>>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.Labels.Where(x => x.BoardId == boardId).Select(x => new LabelResponse(x.Id, x.BoardId, x.Name, x.ColorHex));
        return Task.FromResult(Result<PagedResult<LabelResponse>>.Success(PagedResult<LabelResponse>.Create(query, page, pageSize)));
    }
}
