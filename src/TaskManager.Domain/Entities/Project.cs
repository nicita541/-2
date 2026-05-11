using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public sealed class Project : Entity
{
    public Guid WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public string? CoverUrl { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public bool IsArchived { get; set; }
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<ProjectNote> Notes { get; set; } = new List<ProjectNote>();
}
