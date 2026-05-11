using TaskManager.Application.Columns.Dtos;
using TaskManager.Application.Common;

namespace TaskManager.Application.Columns.Services;

public interface IColumnService
{
    Task<Result<ColumnResponse>> CreateAsync(ColumnCreateRequest request, CancellationToken cancellationToken);
    Task<Result<ColumnResponse>> GetAsync(Guid columnId, CancellationToken cancellationToken);
    Task<Result<ColumnResponse>> UpdateAsync(Guid columnId, ColumnUpdateRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid columnId, CancellationToken cancellationToken);
    Task<Result<PagedResult<ColumnResponse>>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken);
}
