namespace TaskManager.Application.Abstractions;

public interface IPermissionService
{
    Task<bool> CanAccessWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessBoardAsync(Guid boardId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditBoardAsync(Guid boardId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessColumnAsync(Guid columnId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditColumnAsync(Guid columnId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessTaskItemAsync(Guid taskItemId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditTaskItemAsync(Guid taskItemId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessCommentAsync(Guid commentId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessLabelAsync(Guid labelId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanEditLabelAsync(Guid labelId, Guid userId, CancellationToken cancellationToken);
    Task<bool> CanAccessChecklistItemAsync(Guid checklistItemId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsWorkspaceMemberAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsProjectWorkspaceMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken);
}
