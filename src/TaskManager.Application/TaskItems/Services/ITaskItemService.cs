using TaskManager.Application.Common;
using TaskManager.Application.TaskItems.Dtos;

namespace TaskManager.Application.TaskItems.Services;

public interface ITaskItemService
{
    Task<Result<TaskItemResponse>> CreateAsync(TaskItemCreateRequest request, CancellationToken cancellationToken);
    Task<Result<TaskItemResponse>> CreateSubtaskAsync(Guid parentTaskItemId, TaskItemCreateRequest request, CancellationToken cancellationToken);
    Task<Result<TaskItemResponse>> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<TaskItemResponse>> UpdateAsync(Guid id, TaskItemUpdateRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> SoftDeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<TaskItemResponse>> MoveAsync(Guid id, TaskItemMoveRequest request, CancellationToken cancellationToken);
    Task<Result<TaskItemResponse>> MoveAsync(Guid id, TaskItemDragMoveRequest request, CancellationToken cancellationToken);
    Task<Result<PagedResult<TaskItemResponse>>> GetAsync(Guid? projectId, Guid? boardColumnId, Guid? parentTaskItemId, int page, int pageSize, CancellationToken cancellationToken);
}
