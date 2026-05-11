using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Projects.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Projects.Services;

public sealed class ProjectService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions, IDateTimeProvider clock) : IProjectService
{
    public async Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditWorkspaceAsync(request.WorkspaceId, currentUser.UserId, cancellationToken))
        {
            return Result<ProjectResponse>.Failure(Error.Forbidden("You cannot create projects in this workspace."));
        }

        var project = new Project { WorkspaceId = request.WorkspaceId, Name = request.Name, Description = request.Description, Color = request.Color, Icon = request.Icon, CoverUrl = request.CoverUrl, Status = request.Status, IsArchived = request.IsArchived };
        db.Add(project);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ProjectResponse>.Success(Map(project));
    }

    public async Task<Result<PagedResult<ProjectResponse>>> GetByWorkspaceAsync(Guid workspaceId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessWorkspaceAsync(workspaceId, currentUser.UserId, cancellationToken))
        {
            return Result<PagedResult<ProjectResponse>>.Failure(Error.Forbidden("You cannot access this workspace."));
        }

        var query = db.Projects.Where(x => x.WorkspaceId == workspaceId).Select(x => new ProjectResponse(x.Id, x.WorkspaceId, x.Name, x.Description, x.Color, x.Icon, x.CoverUrl, x.Status, x.IsArchived));
        var result = await PagedResult<ProjectResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<ProjectResponse>>.Success(result);
    }

    public async Task<Result<ProjectResponse>> GetAsync(Guid projectId, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessProjectAsync(projectId, currentUser.UserId, cancellationToken))
            return Result<ProjectResponse>.Failure(Error.Forbidden("You cannot access this project."));
        var project = await db.FirstOrDefaultAsync(db.Projects.Where(x => x.Id == projectId), cancellationToken);
        return project is null ? Result<ProjectResponse>.Failure(Error.NotFound("Project not found.")) : Result<ProjectResponse>.Success(Map(project));
    }

    public async Task<Result<ProjectResponse>> UpdateAsync(Guid projectId, ProjectUpdateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditProjectAsync(projectId, currentUser.UserId, cancellationToken))
            return Result<ProjectResponse>.Failure(Error.Forbidden("You cannot edit this project."));
        var project = await db.FirstOrDefaultAsync(db.Projects.Where(x => x.Id == projectId), cancellationToken);
        if (project is null) return Result<ProjectResponse>.Failure(Error.NotFound("Project not found."));
        project.Name = request.Name;
        project.Description = request.Description;
        project.Color = request.Color;
        project.Icon = request.Icon;
        project.CoverUrl = request.CoverUrl;
        project.Status = request.Status;
        project.IsArchived = request.IsArchived;
        await db.SaveChangesAsync(cancellationToken);
        return Result<ProjectResponse>.Success(Map(project));
    }

    private static ProjectResponse Map(Project project) =>
        new(project.Id, project.WorkspaceId, project.Name, project.Description, project.Color, project.Icon, project.CoverUrl, project.Status, project.IsArchived);

    public async Task<Result<bool>> DeleteAsync(Guid projectId, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditProjectAsync(projectId, currentUser.UserId, cancellationToken))
            return Result<bool>.Failure(Error.Forbidden("You cannot delete this project."));
        var project = await db.FirstOrDefaultAsync(db.Projects.Where(x => x.Id == projectId), cancellationToken);
        if (project is null) return Result<bool>.Failure(Error.NotFound("Project not found."));
        project.DeletedAtUtc = clock.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
