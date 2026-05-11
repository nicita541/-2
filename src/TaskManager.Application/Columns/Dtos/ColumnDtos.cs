namespace TaskManager.Application.Columns.Dtos;

public sealed record ColumnCreateRequest(Guid BoardId, string Name, int Position);
public sealed record ColumnResponse(Guid Id, Guid BoardId, string Name, int Position);
