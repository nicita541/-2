using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Attachments.Dtos;
using TaskManager.Application.Attachments.Services;
using TaskManager.Application.Attachments.Validators;
using TaskManager.Application.Auth.Dtos;
using TaskManager.Application.Auth.Validators;
using TaskManager.Application.Boards.Services;
using TaskManager.Application.Boards.Dtos;
using TaskManager.Application.Boards.Validators;
using TaskManager.Application.Checklist.Dtos;
using TaskManager.Application.Checklist.Services;
using TaskManager.Application.Checklist.Validators;
using TaskManager.Application.Columns.Dtos;
using TaskManager.Application.Columns.Services;
using TaskManager.Application.Columns.Validators;
using TaskManager.Application.Comments.Dtos;
using TaskManager.Application.Comments.Services;
using TaskManager.Application.Comments.Validators;
using TaskManager.Application.Common.Validation;
using TaskManager.Application.Labels.Dtos;
using TaskManager.Application.Labels.Services;
using TaskManager.Application.Labels.Validators;
using TaskManager.Application.Projects.Dtos;
using TaskManager.Application.Projects.Services;
using TaskManager.Application.Projects.Validators;
using TaskManager.Application.TaskItems.Dtos;
using TaskManager.Application.TaskItems.Services;
using TaskManager.Application.TaskItems.Validators;
using TaskManager.Application.Workspaces.Dtos;
using TaskManager.Application.Workspaces.Services;
using TaskManager.Application.Workspaces.Validators;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWorkspaceService, WorkspaceService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IColumnService, ColumnService>();
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IChecklistService, ChecklistService>();
        services.AddScoped<IRequestValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IRequestValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IRequestValidator<WorkspaceCreateRequest>, WorkspaceCreateRequestValidator>();
        services.AddScoped<IRequestValidator<AddWorkspaceMemberRequest>, AddWorkspaceMemberRequestValidator>();
        services.AddScoped<IRequestValidator<ProjectCreateRequest>, ProjectCreateRequestValidator>();
        services.AddScoped<IRequestValidator<BoardCreateRequest>, BoardCreateRequestValidator>();
        services.AddScoped<IRequestValidator<ColumnCreateRequest>, ColumnCreateRequestValidator>();
        services.AddScoped<IRequestValidator<TaskItemCreateRequest>, TaskItemCreateRequestValidator>();
        services.AddScoped<IRequestValidator<TaskItemUpdateRequest>, TaskItemUpdateRequestValidator>();
        services.AddScoped<IRequestValidator<TaskItemMoveRequest>, TaskItemMoveRequestValidator>();
        services.AddScoped<IRequestValidator<CommentCreateRequest>, CommentCreateRequestValidator>();
        services.AddScoped<IRequestValidator<AttachmentCreateRequest>, AttachmentCreateRequestValidator>();
        services.AddScoped<IRequestValidator<LabelCreateRequest>, LabelCreateRequestValidator>();
        services.AddScoped<IRequestValidator<ChecklistItemCreateRequest>, ChecklistItemCreateRequestValidator>();
        services.AddScoped<IRequestValidator<ChecklistItemUpdateRequest>, ChecklistItemUpdateRequestValidator>();
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<RegisterRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<LoginRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<WorkspaceCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<AddWorkspaceMemberRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<ProjectCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<BoardCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<ColumnCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<TaskItemCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<TaskItemUpdateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<TaskItemMoveRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<CommentCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<AttachmentCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<LabelCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<ChecklistItemCreateRequest>>());
        services.AddScoped<IRequestValidator>(provider => provider.GetRequiredService<IRequestValidator<ChecklistItemUpdateRequest>>());
        return services;
    }
}
