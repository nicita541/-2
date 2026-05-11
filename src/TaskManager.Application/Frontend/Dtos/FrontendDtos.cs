namespace TaskManager.Application.Frontend.Dtos;

public sealed record ProjectOverviewResponse(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    string? CoverUrl,
    string Status,
    bool IsArchived,
    IReadOnlyList<ProjectOverviewBoardDto> Boards,
    IReadOnlyList<ProjectNoteDto> Notes,
    ProjectStatsDto Stats);

public sealed record ProjectOverviewBoardDto(Guid Id, string Name, int Order, int ColumnsCount, int TasksCount);
public sealed record ProjectNoteDto(Guid Id, string Title, string ContentMarkdown, DateTime CreatedAtUtc);
public sealed record ProjectStatsDto(int TotalTasks, int CompletedTasks, int OverdueTasks);

public sealed record KanbanBoardResponse(Guid Id, Guid ProjectId, string Name, IReadOnlyList<KanbanColumnDto> Columns);
public sealed record KanbanColumnDto(Guid Id, string Name, int Order, IReadOnlyList<KanbanTaskDto> Tasks);
public sealed record KanbanTaskDto(
    Guid Id,
    Guid ProjectId,
    Guid BoardColumnId,
    Guid? ParentTaskItemId,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DeadlineUtc,
    int Order,
    UserSummaryDto? Assignee,
    IReadOnlyList<LabelSummaryDto> Labels,
    CountSummaryDto Checklist,
    CountSummaryDto Subtasks,
    int CommentsCount,
    int AttachmentsCount);

public sealed record TaskDetailsResponse(
    Guid Id,
    Guid ProjectId,
    Guid BoardColumnId,
    Guid? ParentTaskItemId,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DeadlineUtc,
    int Order,
    UserSummaryDto? Assignee,
    IReadOnlyList<LabelSummaryDto> Labels,
    IReadOnlyList<ChecklistItemDetailsDto> ChecklistItems,
    IReadOnlyList<CommentDetailsDto> Comments,
    IReadOnlyList<AttachmentDetailsDto> Attachments,
    IReadOnlyList<KanbanTaskDto> Subtasks);

public sealed record UserSummaryDto(Guid Id, string? DisplayName, string Email);
public sealed record LabelSummaryDto(Guid Id, string Name, string Color);
public sealed record CountSummaryDto(int Total, int Completed);
public sealed record ChecklistItemDetailsDto(Guid Id, string Text, bool IsCompleted, int Order);
public sealed record CommentDetailsDto(Guid Id, Guid AuthorId, string Body, DateTime CreatedAtUtc);
public sealed record AttachmentDetailsDto(Guid Id, string FileName, string ContentType, string Url, long SizeBytes);
