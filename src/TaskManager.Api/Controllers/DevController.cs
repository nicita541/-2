using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Controllers;

public sealed class DevController(IWebHostEnvironment environment, IApplicationDbContext db, TaskManager.Application.Abstractions.IPasswordHasher passwordHasher) : ApiControllerBase
{
    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        var existing = await db.FirstOrDefaultAsync(db.Users.Where(x => x.Email == "demo@taskmanager.local"), cancellationToken);
        if (existing is not null)
        {
            return Ok(new { email = existing.Email, password = "Demo12345!" });
        }

        var user = new ApplicationUser { Email = "demo@taskmanager.local", DisplayName = "Demo User", PasswordHash = string.Empty };
        user.PasswordHash = passwordHasher.HashPassword(user, "Demo12345!");
        db.Add(user);

        var workspace = new Workspace { Name = "Personal", Description = "Demo personal workspace", OwnerId = user.Id, Owner = user };
        workspace.Members.Add(new WorkspaceMember { WorkspaceId = workspace.Id, Workspace = workspace, UserId = user.Id, User = user, Role = WorkspaceRoleType.Owner });
        db.Add(workspace);

        var team = new Workspace { Name = "Team", Description = "Demo team workspace", OwnerId = user.Id, Owner = user };
        team.Members.Add(new WorkspaceMember { WorkspaceId = team.Id, Workspace = team, UserId = user.Id, User = user, Role = WorkspaceRoleType.Owner });
        db.Add(team);

        var project = new Project { WorkspaceId = workspace.Id, Workspace = workspace, Name = "Demo Project", Description = "Frontend-ready demo project" };
        db.Add(project);
        var board = new Board { ProjectId = project.Id, Project = project, Name = "Основная доска" };
        db.Add(board);

        var todo = new BoardColumn { BoardId = board.Id, Board = board, Name = "Нужно сделать", Position = 0 };
        var progress = new BoardColumn { BoardId = board.Id, Board = board, Name = "В процессе", Position = 1 };
        var done = new BoardColumn { BoardId = board.Id, Board = board, Name = "Готово", Position = 2 };
        db.Add(todo);
        db.Add(progress);
        db.Add(done);

        var backend = new Label { BoardId = board.Id, Board = board, Name = "Backend", ColorHex = "#22c55e" };
        var ui = new Label { BoardId = board.Id, Board = board, Name = "UI", ColorHex = "#6366f1" };
        db.Add(backend);
        db.Add(ui);

        var task = new TaskItem { ProjectId = project.Id, Project = project, BoardColumnId = todo.Id, BoardColumn = todo, Title = "Собрать frontend-ready API", Description = "Overview, kanban, details and move endpoints", Position = 0, AssigneeId = user.Id, Assignee = user };
        db.Add(task);
        db.Add(new TaskItem { ProjectId = project.Id, Project = project, BoardColumnId = todo.Id, BoardColumn = todo, ParentTaskItemId = task.Id, ParentTaskItem = task, Title = "Добавить kanban endpoint", Position = 0 });
        db.Add(new TaskItem { ProjectId = project.Id, Project = project, BoardColumnId = progress.Id, BoardColumn = progress, Title = "Сделать React UI", Description = "Vite + React + TS", Position = 0 });
        db.Add(new TaskItem { ProjectId = project.Id, Project = project, BoardColumnId = done.Id, BoardColumn = done, Title = "Стабилизировать permissions", Position = 0 });
        db.Add(new TaskItemLabel { TaskItemId = task.Id, TaskItem = task, LabelId = backend.Id, Label = backend });
        db.Add(new ChecklistItem { TaskItemId = task.Id, TaskItem = task, Text = "Project overview", IsCompleted = true, Position = 0 });
        db.Add(new ChecklistItem { TaskItemId = task.Id, TaskItem = task, Text = "Kanban endpoint", IsCompleted = true, Position = 1 });
        db.Add(new ChecklistItem { TaskItemId = task.Id, TaskItem = task, Text = "Task details", Position = 2 });
        db.Add(new Comment { TaskItemId = task.Id, TaskItem = task, AuthorId = user.Id, Author = user, Body = "Demo comment for frontend." });
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new { email = user.Email, password = "Demo12345!", workspaceId = workspace.Id, projectId = project.Id, boardId = board.Id });
    }
}
