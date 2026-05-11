using TaskManager.Application.Common;
using TaskManager.Application.ProjectNotes.Dtos;

namespace TaskManager.Application.ProjectNotes.Services;

public interface IProjectNoteService
{
    Task<Result<IReadOnlyList<ProjectNoteResponse>>> GetByProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task<Result<ProjectNoteResponse>> CreateAsync(Guid projectId, ProjectNoteCreateRequest request, CancellationToken cancellationToken);
    Task<Result<ProjectNoteResponse>> UpdateAsync(Guid noteId, ProjectNoteCreateRequest request, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid noteId, CancellationToken cancellationToken);
}
