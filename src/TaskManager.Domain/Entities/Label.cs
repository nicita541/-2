using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class Label : Entity
{
    public Guid BoardId { get; set; }
    public Board? Board { get; set; }
    public required string Name { get; set; }
    public required string ColorHex { get; set; }
    public ICollection<TaskItemLabel> TaskItems { get; set; } = new List<TaskItemLabel>();
}
