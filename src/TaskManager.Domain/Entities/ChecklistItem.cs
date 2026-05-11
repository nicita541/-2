using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class ChecklistItem : Entity
{
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public required string Text { get; set; }
    public bool IsCompleted { get; set; }
    public int Position { get; set; }
}
