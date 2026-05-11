using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class ProjectNote : Entity
{
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public required string Title { get; set; }
    public required string ContentMarkdown { get; set; }
}
