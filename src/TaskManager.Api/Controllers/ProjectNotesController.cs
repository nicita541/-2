using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.ProjectNotes.Dtos;
using TaskManager.Application.ProjectNotes.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class ProjectNotesController(IProjectNoteService service) : ApiControllerBase
{
    [HttpGet("/api/projects/{projectId:guid}/notes")]
    public async Task<ActionResult<IReadOnlyList<ProjectNoteResponse>>> Get(Guid projectId, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetByProjectAsync(projectId, cancellationToken));

    [HttpPost("/api/projects/{projectId:guid}/notes")]
    public async Task<ActionResult<ProjectNoteResponse>> Create(Guid projectId, ProjectNoteCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(projectId, request, cancellationToken));

    [HttpPut("/api/project-notes/{noteId:guid}")]
    public async Task<ActionResult<ProjectNoteResponse>> Update(Guid noteId, ProjectNoteCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.UpdateAsync(noteId, request, cancellationToken));

    [HttpDelete("/api/project-notes/{noteId:guid}")]
    public async Task<IActionResult> Delete(Guid noteId, CancellationToken cancellationToken) =>
        ToNoContentResult(await service.DeleteAsync(noteId, cancellationToken));
}
