using TaskManager.Application.Common;
using TaskManager.Application.Frontend.Dtos;

namespace TaskManager.Application.Frontend.Services;

public interface IFrontendReadService
{
    Task<Result<ProjectOverviewResponse>> GetProjectOverviewAsync(Guid projectId, CancellationToken cancellationToken);
    Task<Result<KanbanBoardResponse>> GetBoardKanbanAsync(Guid boardId, CancellationToken cancellationToken);
    Task<Result<TaskDetailsResponse>> GetTaskDetailsAsync(Guid taskItemId, CancellationToken cancellationToken);
}
