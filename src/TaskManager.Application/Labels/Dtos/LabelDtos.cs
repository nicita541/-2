namespace TaskManager.Application.Labels.Dtos;

public sealed record LabelCreateRequest(Guid BoardId, string Name, string ColorHex);
public sealed record LabelResponse(Guid Id, Guid BoardId, string Name, string ColorHex);
