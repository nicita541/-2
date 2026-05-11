using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public sealed class Workspace : Entity
{
    public required string Name { get; set; }
    public WorkspaceType Type { get; set; } = WorkspaceType.Personal;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }
    public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
