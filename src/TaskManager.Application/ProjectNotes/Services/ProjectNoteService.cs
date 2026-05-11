using TaskManager.Application.Abstractions;
using TaskManager.Application.Common;
using TaskManager.Application.ProjectNotes.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.ProjectNotes.Services;

public sealed class ProjectNoteService(IApplicationDbContext db, ICurrentUserService currentUser, IPermissionService permissions, IDateTimeProvider clock) : IProjectNoteService
{
    public async Task<Result<IReadOnlyList<ProjectNoteResponse>>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        if (!await permissions.CanAccessProjectAsync(projectId, currentUser.UserId, cancellationToken))
            return Result<IReadOnlyList<ProjectNoteResponse>>.Failure(Error.Forbidden("You cannot access this project."));
        var notes = await db.ToListAsync(db.ProjectNotes.Where(x => x.ProjectId == projectId).OrderByDescending(x => x.CreatedAtUtc).Select(x => new ProjectNoteResponse(x.Id, x.ProjectId, x.Title, x.ContentMarkdown, x.CreatedAtUtc)), cancellationToken);
        return Result<IReadOnlyList<ProjectNoteResponse>>.Success(notes);
    }

    public async Task<Result<ProjectNoteResponse>> CreateAsync(Guid projectId, ProjectNoteCreateRequest request, CancellationToken cancellationToken)
    {
        if (!await permissions.CanEditProjectAsync(projectId, currentUser.UserId, cancellationToken))
            return Result<ProjectNoteResponse>.Failure(Error.Forbidden("You cannot edit this project."));
        var note = new ProjectNote { ProjectId = projectId, Title = request.Title, ContentMarkdown = request.ContentMarkdown };
        db.Add(note);
        await db.SaveChangesAsync(cancellationToken);
        return Result<ProjectNoteResponse>.Success(new ProjectNoteResponse(note.Id, note.ProjectId, note.Title, note.ContentMarkdown, note.CreatedAtUtc));
    }

    public async Task<Result<ProjectNoteResponse>> UpdateAsync(Guid noteId, ProjectNoteCreateRequest request, CancellationToken cancellationToken)
    {
        var note = await db.FirstOrDefaultAsync(db.ProjectNotes.Where(x => x.Id == noteId), cancellationToken);
        if (note is null) return Result<ProjectNoteResponse>.Failure(Error.NotFound("Project note not found."));
        if (!await permissions.CanEditProjectAsync(note.ProjectId, currentUser.UserId, cancellationToken))
            return Result<ProjectNoteResponse>.Failure(Error.Forbidden("You cannot edit this project."));
        note.Title = request.Title;
        note.ContentMarkdown = request.ContentMarkdown;
        await db.SaveChangesAsync(cancellationToken);
        return Result<ProjectNoteResponse>.Success(new ProjectNoteResponse(note.Id, note.ProjectId, note.Title, note.ContentMarkdown, note.CreatedAtUtc));
    }

    public async Task<Result<bool>> DeleteAsync(Guid noteId, CancellationToken cancellationToken)
    {
        var note = await db.FirstOrDefaultAsync(db.ProjectNotes.Where(x => x.Id == noteId), cancellationToken);
        if (note is null) return Result<bool>.Failure(Error.NotFound("Project note not found."));
        if (!await permissions.CanEditProjectAsync(note.ProjectId, currentUser.UserId, cancellationToken))
            return Result<bool>.Failure(Error.Forbidden("You cannot delete this project note."));
        note.DeletedAtUtc = clock.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
