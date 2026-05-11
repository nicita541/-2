using TaskManager.Application.Abstractions;
using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Api.IntegrationTests;

internal sealed class InMemoryApplicationDbContext : IApplicationDbContext
{
    private readonly List<ApplicationUser> _users = [];
    private readonly List<Workspace> _workspaces = [];
    private readonly List<WorkspaceMember> _workspaceMembers = [];
    private readonly List<Project> _projects = [];
    private readonly List<Board> _boards = [];
    private readonly List<BoardColumn> _columns = [];
    private readonly List<TaskItem> _taskItems = [];
    private readonly List<Comment> _comments = [];
    private readonly List<Attachment> _attachments = [];
    private readonly List<Label> _labels = [];
    private readonly List<TaskItemLabel> _taskItemLabels = [];
    private readonly List<ChecklistItem> _checklistItems = [];

    public IQueryable<ApplicationUser> Users => _users.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<Workspace> Workspaces => _workspaces.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<WorkspaceMember> WorkspaceMembers => _workspaceMembers.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<Project> Projects => _projects.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<Board> Boards => _boards.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<BoardColumn> BoardColumns => _columns.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<TaskItem> TaskItems => _taskItems.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<Comment> Comments => _comments.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<Attachment> Attachments => _attachments.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<Label> Labels => _labels.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<TaskItemLabel> TaskItemLabels => _taskItemLabels.Where(x => x.DeletedAtUtc == null).AsQueryable();
    public IQueryable<ChecklistItem> ChecklistItems => _checklistItems.Where(x => x.DeletedAtUtc == null).AsQueryable();

    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        Stamp(entity);

        switch (entity)
        {
            case ApplicationUser user:
                _users.Add(user);
                break;
            case Workspace workspace:
                _workspaces.Add(workspace);
                foreach (var member in workspace.Members)
                {
                    member.WorkspaceId = workspace.Id;
                    member.Workspace = workspace;
                    member.User = _users.FirstOrDefault(x => x.Id == member.UserId);
                    _workspaceMembers.Add(member);
                }
                break;
            case WorkspaceMember member:
                member.Workspace = _workspaces.FirstOrDefault(x => x.Id == member.WorkspaceId);
                member.User = _users.FirstOrDefault(x => x.Id == member.UserId);
                _workspaceMembers.Add(member);
                break;
            case Project project:
                project.Workspace = _workspaces.FirstOrDefault(x => x.Id == project.WorkspaceId);
                _projects.Add(project);
                break;
            case Board board:
                board.Project = _projects.FirstOrDefault(x => x.Id == board.ProjectId);
                _boards.Add(board);
                break;
            case BoardColumn column:
                column.Board = _boards.FirstOrDefault(x => x.Id == column.BoardId);
                _columns.Add(column);
                break;
            case TaskItem taskItem:
                taskItem.Project = _projects.FirstOrDefault(x => x.Id == taskItem.ProjectId);
                taskItem.BoardColumn = _columns.FirstOrDefault(x => x.Id == taskItem.BoardColumnId);
                taskItem.ParentTaskItem = _taskItems.FirstOrDefault(x => x.Id == taskItem.ParentTaskItemId);
                _taskItems.Add(taskItem);
                break;
            case Comment comment:
                comment.TaskItem = _taskItems.FirstOrDefault(x => x.Id == comment.TaskItemId);
                comment.Author = _users.FirstOrDefault(x => x.Id == comment.AuthorId);
                _comments.Add(comment);
                break;
            case Attachment attachment:
                attachment.TaskItem = _taskItems.FirstOrDefault(x => x.Id == attachment.TaskItemId);
                attachment.UploadedBy = _users.FirstOrDefault(x => x.Id == attachment.UploadedById);
                _attachments.Add(attachment);
                break;
            case Label label:
                label.Board = _boards.FirstOrDefault(x => x.Id == label.BoardId);
                _labels.Add(label);
                break;
            case TaskItemLabel taskItemLabel:
                taskItemLabel.TaskItem = _taskItems.FirstOrDefault(x => x.Id == taskItemLabel.TaskItemId);
                taskItemLabel.Label = _labels.FirstOrDefault(x => x.Id == taskItemLabel.LabelId);
                _taskItemLabels.Add(taskItemLabel);
                break;
            case ChecklistItem checklistItem:
                checklistItem.TaskItem = _taskItems.FirstOrDefault(x => x.Id == checklistItem.TaskItemId);
                _checklistItems.Add(checklistItem);
                break;
            default:
                throw new InvalidOperationException($"Unsupported entity type {typeof(TEntity).Name}.");
        }
    }

    public Task<bool> AnyAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        Task.FromResult(query.Any());

    public Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        Task.FromResult(query.FirstOrDefault());

    public Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        Task.FromResult(query.ToList());

    public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken cancellationToken = default) =>
        Task.FromResult(query.Count());

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entity in AllEntities().OfType<IAuditableEntity>())
        {
            if (entity.CreatedAtUtc == default)
            {
                entity.CreatedAtUtc = DateTime.UtcNow;
            }
        }

        return Task.FromResult(1);
    }

    private IEnumerable<object> AllEntities() =>
        _users.Cast<object>()
            .Concat(_workspaces)
            .Concat(_workspaceMembers)
            .Concat(_projects)
            .Concat(_boards)
            .Concat(_columns)
            .Concat(_taskItems)
            .Concat(_comments)
            .Concat(_attachments)
            .Concat(_labels)
            .Concat(_taskItemLabels)
            .Concat(_checklistItems);

    private static void Stamp(object entity)
    {
        if (entity is IAuditableEntity auditable && auditable.CreatedAtUtc == default)
        {
            auditable.CreatedAtUtc = DateTime.UtcNow;
        }
    }
}
