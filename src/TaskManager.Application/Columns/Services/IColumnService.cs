using TaskManager.Application.Columns.Dtos;
using TaskManager.Application.Common;

namespace TaskManager.Application.Columns.Services;

public interface IColumnService
{
    Task<Result<ColumnResponse>> CreateAsync(ColumnCreateRequest request, CancellationToken cancellationToken);
    Task<Result<PagedResult<ColumnResponse>>> GetByBoardAsync(Guid boardId, int page, int pageSize, CancellationToken cancellationToken);
}
