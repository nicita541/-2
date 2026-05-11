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
    Task<bool> AnyAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default);
    Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default);
    Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
