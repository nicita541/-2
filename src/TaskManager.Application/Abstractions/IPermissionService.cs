namespace TaskManager.Application.Abstractions;

public interface IPermissionService
{
    Task<bool> CanAccessWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
}
