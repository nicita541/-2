using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class BoardColumn : Entity
{
    public Guid BoardId { get; set; }
    public Board? Board { get; set; }
    public required string Name { get; set; }
    public int Position { get; set; }
    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
