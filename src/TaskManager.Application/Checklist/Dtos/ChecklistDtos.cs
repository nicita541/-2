namespace TaskManager.Application.Checklist.Dtos;

public sealed record ChecklistItemCreateRequest(Guid TaskItemId, string Text, int Position);
public sealed record ChecklistItemUpdateRequest(string Text, bool IsCompleted, int Position);
public sealed record ChecklistItemResponse(Guid Id, Guid TaskItemId, string Text, bool IsCompleted, int Position);
