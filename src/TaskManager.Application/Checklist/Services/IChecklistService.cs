using TaskManager.Application.Checklist.Dtos;
using TaskManager.Application.Common;

namespace TaskManager.Application.Checklist.Services;

public interface IChecklistService
{
    Task<Result<ChecklistItemResponse>> CreateAsync(ChecklistItemCreateRequest request, CancellationToken cancellationToken);
    Task<Result<ChecklistItemResponse>> UpdateAsync(Guid id, ChecklistItemUpdateRequest request, CancellationToken cancellationToken);
    Task<Result<ChecklistItemResponse>> ToggleAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
