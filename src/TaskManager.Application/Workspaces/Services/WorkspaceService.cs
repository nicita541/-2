using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.Workspaces.Dtos;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Workspaces.Services;

public sealed class WorkspaceService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions) : IWorkspaceService
{
    public async Task<Result<WorkspaceResponse>> CreateAsync(WorkspaceCreateRequest request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;
        var workspace = new Workspace { Name = request.Name, Description = request.Description, OwnerId = userId };
        workspace.Members.Add(new WorkspaceMember { UserId = userId, Role = WorkspaceRoleType.Owner });
        db.Add(workspace);
        await db.SaveChangesAsync(cancellationToken);
        return Result<WorkspaceResponse>.Success(new WorkspaceResponse(workspace.Id, workspace.Name, workspace.Description, workspace.OwnerId));
    }

    public async Task<Result<PagedResult<WorkspaceResponse>>> GetMineAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.WorkspaceMembers
            .Where(x => x.UserId == currentUser.UserId)
            .Select(x => new WorkspaceResponse(x.Workspace!.Id, x.Workspace.Name, x.Workspace.Description, x.Workspace.OwnerId));

        var result = await PagedResult<WorkspaceResponse>.CreateAsync(query, page, pageSize, db.CountAsync, db.ToListAsync, cancellationToken);
        return Result<PagedResult<WorkspaceResponse>>.Success(result);
    }

    public async Task<Result<WorkspaceMemberResponse>> AddMemberAsync(Guid workspaceId, AddWorkspaceMemberRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditWorkspaceAsync(workspaceId, currentUser.UserId, cancellationToken))
        {
            return Result<WorkspaceMemberResponse>.Failure(Error.Forbidden("You cannot edit this workspace."));
        }

        var exists = await db.AnyAsync(db.WorkspaceMembers.Where(x => x.WorkspaceId == workspaceId && x.UserId == request.UserId), cancellationToken);
        if (exists) return Result<WorkspaceMemberResponse>.Failure(Error.Conflict("User is already a workspace member."));

        var userExists = await db.AnyAsync(db.Users.Where(x => x.Id == request.UserId), cancellationToken);
        if (!userExists) return Result<WorkspaceMemberResponse>.Failure(Error.NotFound("User not found."));

        var member = new WorkspaceMember { WorkspaceId = workspaceId, UserId = request.UserId, Role = request.Role };
        db.Add(member);
        await db.SaveChangesAsync(cancellationToken);
        return Result<WorkspaceMemberResponse>.Success(new WorkspaceMemberResponse(member.Id, member.UserId, member.Role));
    }
}
