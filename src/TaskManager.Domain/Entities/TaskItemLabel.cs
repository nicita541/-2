using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class TaskItemLabel : Entity
{
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public Guid LabelId { get; set; }
    public Label? Label { get; set; }
}
