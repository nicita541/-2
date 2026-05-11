using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public sealed class Workspace : Entity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }
    public ICollection<WorkspaceMember> Members { get; set; } = new List<WorkspaceMember>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
