using TaskManager.Application.Common;
using TaskManager.Application.Workspaces.Dtos;

namespace TaskManager.Application.Workspaces.Services;

public interface IWorkspaceService
{
    Task<Result<WorkspaceResponse>> CreateAsync(WorkspaceCreateRequest request, CancellationToken cancellationToken);
    Task<Result<WorkspaceResponse>> GetAsync(Guid workspaceId, CancellationToken cancellationToken);
    Task<Result<WorkspaceResponse>> UpdateAsync(Guid workspaceId, WorkspaceUpdateRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid workspaceId, CancellationToken cancellationToken);
    Task<Result<PagedResult<WorkspaceResponse>>> GetMineAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Result<WorkspaceMemberResponse>> AddMemberAsync(Guid workspaceId, AddWorkspaceMemberRequest request, CancellationToken cancellationToken);
}
