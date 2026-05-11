using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Frontend.Dtos;

namespace TaskManager.Application.Frontend.Services;

public sealed class FrontendReadService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : IFrontendReadService
{
    public async Task<Result<ProjectOverviewResponse>> GetProjectOverviewAsync(Guid projectId, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessProjectAsync(projectId, currentUser.UserId, cancellationToken))
            return Result<ProjectOverviewResponse>.Failure(Error.Forbidden("You cannot access this project."));

        var project = await db.FirstOrDefaultAsync(db.Projects.Where(x => x.Id == projectId), cancellationToken);
        if (project is null) return Result<ProjectOverviewResponse>.Failure(Error.NotFound("Project not found."));

        var boards = await db.ToListAsync(db.Boards.Where(x => x.ProjectId == projectId).Select(x => new ProjectOverviewBoardDto(
            x.Id,
            x.Name,
            0,
            db.BoardColumns.Count(c => c.BoardId == x.Id),
            db.TaskItems.Count(t => t.ProjectId == projectId && t.BoardColumn!.BoardId == x.Id))), cancellationToken);

        var totalTasks = await db.CountAsync(db.TaskItems.Where(x => x.ProjectId == projectId), cancellationToken);
        var overdueTasks = await db.CountAsync(db.TaskItems.Where(x => x.ProjectId == projectId && x.DueDateUtc != null && x.DueDateUtc < DateTime.UtcNow), cancellationToken);

        return Result<ProjectOverviewResponse>.Success(new ProjectOverviewResponse(
            project.Id,
            project.WorkspaceId,
            project.Name,
            project.Description,
            "#6366f1",
            "folder",
            null,
            "Active",
            false,
            boards,
            [],
            new ProjectStatsDto(totalTasks, 0, overdueTasks)));
    }

    public async Task<Result<KanbanBoardResponse>> GetBoardKanbanAsync(Guid boardId, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessBoardAsync(boardId, currentUser.UserId, cancellationToken))
            return Result<KanbanBoardResponse>.Failure(Error.Forbidden("You cannot access this board."));

        var board = await db.FirstOrDefaultAsync(db.Boards.Where(x => x.Id == boardId), cancellationToken);
        if (board is null) return Result<KanbanBoardResponse>.Failure(Error.NotFound("Board not found."));

        var columns = await db.ToListAsync(db.BoardColumns.Where(x => x.BoardId == boardId).OrderBy(x => x.Position), cancellationToken);
        var columnDtos = new List<KanbanColumnDto>();
        foreach (var column in columns)
        {
            var tasks = await db.ToListAsync(db.TaskItems.Where(x => x.BoardColumnId == column.Id && x.ParentTaskItemId == null).OrderBy(x => x.Position), cancellationToken);
            columnDtos.Add(new KanbanColumnDto(column.Id, column.Name, column.Position, tasks.Select(MapKanbanTask).ToList()));
        }

        return Result<KanbanBoardResponse>.Success(new KanbanBoardResponse(board.Id, board.ProjectId, board.Name, columnDtos));
    }

    public async Task<Result<TaskDetailsResponse>> GetTaskDetailsAsync(Guid taskItemId, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessTaskItemAsync(taskItemId, currentUser.UserId, cancellationToken))
            return Result<TaskDetailsResponse>.Failure(Error.Forbidden("You cannot access this task item."));

        var task = await db.FirstOrDefaultAsync(db.TaskItems.Where(x => x.Id == taskItemId), cancellationToken);
        if (task is null) return Result<TaskDetailsResponse>.Failure(Error.NotFound("Task item not found."));

        var labels = await db.ToListAsync(db.TaskItemLabels.Where(x => x.TaskItemId == task.Id).Select(x => new LabelSummaryDto(x.Label!.Id, x.Label.Name, x.Label.ColorHex)), cancellationToken);
        var checklist = await db.ToListAsync(db.ChecklistItems.Where(x => x.TaskItemId == task.Id).OrderBy(x => x.Position).Select(x => new ChecklistItemDetailsDto(x.Id, x.Text, x.IsCompleted, x.Position)), cancellationToken);
        var comments = await db.ToListAsync(db.Comments.Where(x => x.TaskItemId == task.Id).OrderBy(x => x.CreatedAtUtc).Select(x => new CommentDetailsDto(x.Id, x.AuthorId, x.Body, x.CreatedAtUtc)), cancellationToken);
        var attachments = await db.ToListAsync(db.Attachments.Where(x => x.TaskItemId == task.Id).Select(x => new AttachmentDetailsDto(x.Id, x.FileName, x.ContentType, x.Url, x.SizeBytes)), cancellationToken);
        var subtasks = await db.ToListAsync(db.TaskItems.Where(x => x.ParentTaskItemId == task.Id).OrderBy(x => x.Position), cancellationToken);

        return Result<TaskDetailsResponse>.Success(new TaskDetailsResponse(
            task.Id,
            task.ProjectId,
            task.BoardColumnId,
            task.ParentTaskItemId,
            task.Title,
            task.Description,
            "Todo",
            "Medium",
            task.DueDateUtc,
            task.Position,
            MapUser(task.Assignee),
            labels,
            checklist,
            comments,
            attachments,
            subtasks.Select(MapKanbanTask).ToList()));
    }

    private KanbanTaskDto MapKanbanTask(TaskManager.Domain.Entities.TaskItem task)
    {
        var checklistTotal = db.ChecklistItems.Count(x => x.TaskItemId == task.Id);
        var checklistCompleted = db.ChecklistItems.Count(x => x.TaskItemId == task.Id && x.IsCompleted);
        var subtasksTotal = db.TaskItems.Count(x => x.ParentTaskItemId == task.Id);

        return new KanbanTaskDto(
            task.Id,
            task.ProjectId,
            task.BoardColumnId,
            task.ParentTaskItemId,
            task.Title,
            task.Description,
            "Todo",
            "Medium",
            task.DueDateUtc,
            task.Position,
            MapUser(task.Assignee),
            db.TaskItemLabels.Where(x => x.TaskItemId == task.Id).Select(x => new LabelSummaryDto(x.Label!.Id, x.Label.Name, x.Label.ColorHex)).ToList(),
            new CountSummaryDto(checklistTotal, checklistCompleted),
            new CountSummaryDto(subtasksTotal, 0),
            db.Comments.Count(x => x.TaskItemId == task.Id),
            db.Attachments.Count(x => x.TaskItemId == task.Id));
    }

    private static UserSummaryDto? MapUser(TaskManager.Domain.Entities.ApplicationUser? user) =>
        user is null ? null : new UserSummaryDto(user.Id, user.DisplayName, user.Email);
}
