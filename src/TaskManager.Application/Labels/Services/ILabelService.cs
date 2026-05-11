using TaskManager.Application.Common;
using TaskManager.Application.Labels.Dtos;

namespace TaskManager.Application.Labels.Services;

public interface ILabelService
{
    Task<Result<LabelResponse>> CreateAsync(LabelCreateRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> AssignAsync(Guid taskItemId, Guid labelId, CancellationToken cancellationToken);
    Task<Result<PagedResult<LabelResponse>>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken);
}
