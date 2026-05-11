namespace TaskManager.Application.Projects.Dtos;

public sealed record ProjectCreateRequest(Guid WorkspaceId, string Name, string? Description);
public sealed record ProjectResponse(Guid Id, Guid WorkspaceId, string Name, string? Description);
