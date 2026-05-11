using TaskManager.Application.Abstractions;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Permissions;

public sealed class PermissionService(IApplicationDbContext db) : IPermissionService
{
    public Task<bool> CanAccessWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(db.WorkspaceMembers.Any(x => x.WorkspaceId == workspaceId && x.UserId == userId));

    public Task<bool> CanEditWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken) =>
        Task.FromResult(db.WorkspaceMembers.Any(x =>
            x.WorkspaceId == workspaceId &&
            x.UserId == userId &&
            (x.Role == WorkspaceRoleType.Owner || x.Role == WorkspaceRoleType.Admin)));

    public Task<bool> CanAccessProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var workspaceId = db.Projects.Where(x => x.Id == projectId).Select(x => x.WorkspaceId).FirstOrDefault();
        return workspaceId == Guid.Empty ? Task.FromResult(false) : CanAccessWorkspaceAsync(workspaceId, userId, cancellationToken);
    }

    public Task<bool> CanEditProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var workspaceId = db.Projects.Where(x => x.Id == projectId).Select(x => x.WorkspaceId).FirstOrDefault();
        return workspaceId == Guid.Empty ? Task.FromResult(false) : CanEditWorkspaceAsync(workspaceId, userId, cancellationToken);
    }
}
