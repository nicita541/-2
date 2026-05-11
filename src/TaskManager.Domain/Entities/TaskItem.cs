using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class TaskItem : Entity
{
    public Guid BoardColumnId { get; set; }
    public BoardColumn? BoardColumn { get; set; }
    public Guid? ParentTaskItemId { get; set; }
    public TaskItem? ParentTaskItem { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int Position { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public Guid? AssigneeId { get; set; }
    public ApplicationUser? Assignee { get; set; }
    public ICollection<TaskItem> Subtasks { get; set; } = new List<TaskItem>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    public ICollection<ChecklistItem> Checklist { get; set; } = new List<ChecklistItem>();
    public ICollection<TaskItemLabel> Labels { get; set; } = new List<TaskItemLabel>();
}
