using TaskManager.Application.Abstractions;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Permissions;

public sealed class PermissionService(IApplicationDbContext db) : IPermissionService
{
    public Task<bool> CanAccessWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken) =>
        IsWorkspaceMemberAsync(workspaceId, userId, cancellationToken);

    public async Task<bool> CanEditWorkspaceAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken) =>
        await db.AnyAsync(db.WorkspaceMembers.Where(x =>
            x.WorkspaceId == workspaceId &&
            x.UserId == userId &&
            (x.Role == WorkspaceRoleType.Owner || x.Role == WorkspaceRoleType.Admin)), cancellationToken);

    public async Task<bool> CanAccessProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var workspaceId = await db.FirstOrDefaultAsync(db.Projects.Where(x => x.Id == projectId).Select(x => x.WorkspaceId), cancellationToken);
        return workspaceId != Guid.Empty && await CanAccessWorkspaceAsync(workspaceId, userId, cancellationToken);
    }

    public async Task<bool> CanEditProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var workspaceId = await db.FirstOrDefaultAsync(db.Projects.Where(x => x.Id == projectId).Select(x => x.WorkspaceId), cancellationToken);
        return workspaceId != Guid.Empty && await CanEditWorkspaceAsync(workspaceId, userId, cancellationToken);
    }

    public async Task<bool> CanAccessBoardAsync(Guid boardId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.Boards.Where(x => x.Id == boardId).Select(x => x.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanAccessProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanEditBoardAsync(Guid boardId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.Boards.Where(x => x.Id == boardId).Select(x => x.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanEditProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanAccessColumnAsync(Guid columnId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(
            db.BoardColumns.Where(x => x.Id == columnId).Select(x => x.Board!.ProjectId),
            cancellationToken);

        return projectId != Guid.Empty && await CanAccessProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanEditColumnAsync(Guid columnId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(
            db.BoardColumns.Where(x => x.Id == columnId).Select(x => x.Board!.ProjectId),
            cancellationToken);

        return projectId != Guid.Empty && await CanEditProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanAccessTaskItemAsync(Guid taskItemId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == taskItemId).Select(x => x.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanAccessProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanEditTaskItemAsync(Guid taskItemId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == taskItemId).Select(x => x.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanEditProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanAccessCommentAsync(Guid commentId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.Comments.Where(x => x.Id == commentId).Select(x => x.TaskItem!.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanAccessProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanAccessLabelAsync(Guid labelId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.Labels.Where(x => x.Id == labelId).Select(x => x.Board!.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanAccessProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanEditLabelAsync(Guid labelId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.Labels.Where(x => x.Id == labelId).Select(x => x.Board!.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanEditProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> CanAccessChecklistItemAsync(Guid checklistItemId, Guid userId, CancellationToken cancellationToken)
    {
        var projectId = await db.FirstOrDefaultAsync(db.ChecklistItems.Where(x => x.Id == checklistItemId).Select(x => x.TaskItem!.ProjectId), cancellationToken);
        return projectId != Guid.Empty && await CanAccessProjectAsync(projectId, userId, cancellationToken);
    }

    public async Task<bool> IsWorkspaceMemberAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken) =>
        await db.AnyAsync(db.WorkspaceMembers.Where(x => x.WorkspaceId == workspaceId && x.UserId == userId), cancellationToken);

    public async Task<bool> IsProjectWorkspaceMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken)
    {
        var workspaceId = await db.FirstOrDefaultAsync(db.Projects.Where(x => x.Id == projectId).Select(x => x.WorkspaceId), cancellationToken);
        return workspaceId != Guid.Empty && await IsWorkspaceMemberAsync(workspaceId, userId, cancellationToken);
    }
}
