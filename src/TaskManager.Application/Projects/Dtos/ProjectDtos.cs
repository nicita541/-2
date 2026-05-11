using TaskManager.Domain.Enums;

namespace TaskManager.Application.Projects.Dtos;

public sealed record ProjectCreateRequest(Guid WorkspaceId, string Name, string? Description, string? Color = null, string? Icon = null, string? CoverUrl = null, ProjectStatus Status = ProjectStatus.Active, bool IsArchived = false);
public sealed record ProjectUpdateRequest(string Name, string? Description, string? Color = null, string? Icon = null, string? CoverUrl = null, ProjectStatus Status = ProjectStatus.Active, bool IsArchived = false);
public sealed record ProjectResponse(Guid Id, Guid WorkspaceId, string Name, string? Description, string? Color, string? Icon, string? CoverUrl, ProjectStatus Status, bool IsArchived);
