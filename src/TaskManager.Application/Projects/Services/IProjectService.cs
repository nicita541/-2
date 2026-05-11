using TaskManager.Application.Common;
using TaskManager.Application.Projects.Dtos;

namespace TaskManager.Application.Projects.Services;

public interface IProjectService
{
    Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request, CancellationToken cancellationToken);
    Task<Result<ProjectResponse>> GetAsync(Guid projectId, CancellationToken cancellationToken);
    Task<Result<ProjectResponse>> UpdateAsync(Guid projectId, ProjectUpdateRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid projectId, CancellationToken cancellationToken);
    Task<Result<PagedResult<ProjectResponse>>> GetByWorkspaceAsync(Guid workspaceId, int page, int pageSize, CancellationToken cancellationToken);
}
