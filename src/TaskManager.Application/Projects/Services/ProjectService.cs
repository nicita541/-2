using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Projects.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Projects.Services;

public sealed class ProjectService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : IProjectService
{
    public async Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditWorkspaceAsync(request.WorkspaceId, currentUser.UserId, cancellationToken))
        {
            return Result<ProjectResponse>.Failure(Error.Forbidden("You cannot create projects in this workspace."));
        }

        var project = new Project { WorkspaceId = request.WorkspaceId, Name = request.Name, Description = request.Description };
        db.Add(project);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ProjectResponse>.Success(new ProjectResponse(project.Id, project.WorkspaceId, project.Name, project.Description));
    }

    public async Task<Result<PagedResult<ProjectResponse>>> GetByWorkspaceAsync(Guid workspaceId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessWorkspaceAsync(workspaceId, currentUser.UserId, cancellationToken))
        {
            return Result<PagedResult<ProjectResponse>>.Failure(Error.Forbidden("You cannot access this workspace."));
        }

        var query = db.Projects.Where(x => x.WorkspaceId == workspaceId).Select(x => new ProjectResponse(x.Id, x.WorkspaceId, x.Name, x.Description));
        var result = await PagedResult<ProjectResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<ProjectResponse>>.Success(result);
    }
}
