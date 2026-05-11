using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions;

public interface IApplicationDbContext
{
    IQueryable<ApplicationUser> Users { get; }
    IQueryable<Workspace> Workspaces { get; }
    IQueryable<WorkspaceMember> WorkspaceMembers { get; }
    IQueryable<Project> Projects { get; }
    IQueryable<Board> Boards { get; }
    IQueryable<BoardColumn> BoardColumns { get; }
    IQueryable<TaskItem> TaskItems { get; }
    IQueryable<Comment> Comments { get; }
    IQueryable<Attachment> Attachments { get; }
    IQueryable<Label> Labels { get; }
    IQueryable<TaskItemLabel> TaskItemLabels { get; }
    IQueryable<ChecklistItem> ChecklistItems { get; }
    void Add<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
