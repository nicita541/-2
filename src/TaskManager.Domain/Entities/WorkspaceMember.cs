using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public sealed class WorkspaceMember : Entity
{
    public Guid WorkspaceId { get; set; }
    public Workspace? Workspace { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public WorkspaceRoleType Role { get; set; } = WorkspaceRoleType.Member;
}
