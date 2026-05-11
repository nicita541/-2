using TaskManager.Application.Common;
using TaskManager.Application.Projects.Dtos;

namespace TaskManager.Application.Projects.Services;

public interface IProjectService
{
    Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request, CancellationToken cancellationToken);
    Task<Result<PagedResult<ProjectResponse>>> GetByWorkspaceAsync(Guid workspaceId, int page, int pageSize, CancellationToken cancellationToken);
}
