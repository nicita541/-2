using TaskManager.Domain.Enums;

namespace TaskManager.Application.Workspaces.Dtos;

public sealed record WorkspaceCreateRequest(string Name, string? Description);
public sealed record WorkspaceResponse(Guid Id, string Name, string? Description, Guid OwnerId);
public sealed record WorkspaceMemberResponse(Guid Id, Guid UserId, WorkspaceRoleType Role);
public sealed record AddWorkspaceMemberRequest(Guid UserId, WorkspaceRoleType Role);
