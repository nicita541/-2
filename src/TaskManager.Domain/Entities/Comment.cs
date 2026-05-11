using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class Comment : Entity
{
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public Guid AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }
    public required string Body { get; set; }
}
