namespace TaskManager.Application.Auth.Dtos;

public sealed record RegisterRequest(string Email, string Password, string? DisplayName);
public sealed record LoginRequest(string Email, string Password);
public sealed record AuthUserDto(Guid Id, string Email, string? DisplayName);
public sealed record AuthWorkspaceDto(Guid Id, string Name, string Type, string Role);
public sealed record AuthResponse(Guid UserId, string Email, string? DisplayName, string AccessToken, AuthUserDto User, IReadOnlyList<AuthWorkspaceDto> Workspaces);
public sealed record CurrentUserResponse(Guid Id, string Email, string? DisplayName, IReadOnlyList<AuthWorkspaceDto> Workspaces);
