using TaskManager.Domain.Enums;

namespace TaskManager.Application.TaskItems.Dtos;

public sealed record TaskItemCreateRequest(Guid ProjectId, Guid BoardColumnId, Guid? ParentTaskItemId, string Title, string? Description, int Position, DateTime? DueDateUtc, Guid? AssigneeId, TaskItemStatus Status = TaskItemStatus.Todo, TaskPriority Priority = TaskPriority.Medium);
public sealed record TaskItemUpdateRequest(string Title, string? Description, int Position, DateTime? DueDateUtc, Guid? AssigneeId, Guid ProjectId, Guid BoardColumnId, TaskItemStatus Status = TaskItemStatus.Todo, TaskPriority Priority = TaskPriority.Medium);
public sealed record TaskItemResponse(Guid Id, Guid ProjectId, Guid BoardColumnId, Guid? ParentTaskItemId, string Title, string? Description, int Position, DateTime? DueDateUtc, Guid? AssigneeId, TaskItemStatus Status, TaskPriority Priority);
public sealed record TaskItemMoveRequest(Guid BoardColumnId, int Position);
public sealed record TaskItemDragMoveRequest(Guid TargetBoardColumnId, Guid? TargetParentTaskItemId, int NewOrder);
