using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Projects.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Projects.Services;

public sealed class ProjectService(IApplicationDbContext db) : IProjectService
{
    public async Task<Result<ProjectResponse>> CreateAsync(ProjectCreateRequest request, CancellationToken cancellationToken)
    {
        var project = new Project { WorkspaceId = request.WorkspaceId, Name = request.Name, Description = request.Description };
        db.Add(project);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ProjectResponse>.Success(new ProjectResponse(project.Id, project.WorkspaceId, project.Name, project.Description));
    }

    public Task<Result<PagedResult<ProjectResponse>>> GetByWorkspaceAsync(Guid workspaceId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.Projects.Where(x => x.WorkspaceId == workspaceId).Select(x => new ProjectResponse(x.Id, x.WorkspaceId, x.Name, x.Description));
        return Task.FromResult(Result<PagedResult<ProjectResponse>>.Success(PagedResult<ProjectResponse>.Create(query, page, pageSize)));
    }
}
