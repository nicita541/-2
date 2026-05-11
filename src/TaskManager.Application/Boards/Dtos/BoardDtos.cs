namespace TaskManager.Application.Boards.Dtos;

public sealed record BoardCreateRequest(Guid ProjectId, string Name);
public sealed record BoardResponse(Guid Id, Guid ProjectId, string Name);
