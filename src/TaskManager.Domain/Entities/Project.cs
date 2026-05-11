using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class Project : Entity
{
    public Guid WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Board> Boards { get; set; } = new List<Board>();
}
