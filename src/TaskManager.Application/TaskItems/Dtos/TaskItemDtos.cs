namespace TaskManager.Application.TaskItems.Dtos;

public sealed record TaskItemCreateRequest(Guid ProjectId, Guid BoardColumnId, Guid? ParentTaskItemId, string Title, string? Description, int Position, DateTime? DueDateUtc, Guid? AssigneeId);
public sealed record TaskItemUpdateRequest(string Title, string? Description, int Position, DateTime? DueDateUtc, Guid? AssigneeId, Guid ProjectId, Guid BoardColumnId);
public sealed record TaskItemResponse(Guid Id, Guid ProjectId, Guid BoardColumnId, Guid? ParentTaskItemId, string Title, string? Description, int Position, DateTime? DueDateUtc, Guid? AssigneeId);
public sealed record TaskItemMoveRequest(Guid BoardColumnId, int Position);
