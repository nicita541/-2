using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;
using TaskManager.Application.Labels.Dtos;
using TaskManager.Application.Labels.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class LabelsController(ILabelService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<LabelResponse>>> Get([FromQuery] Guid boardId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        ToActionResult(await service.GetByBoardAsync(boardId, page, pageSize, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<LabelResponse>> Create(LabelCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));

    [HttpGet("/api/projects/{projectId:guid}/labels")]
    public async Task<ActionResult<IReadOnlyList<LabelResponse>>> GetProjectLabels(Guid projectId, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetByProjectAsync(projectId, cancellationToken));

    [HttpPost("/api/projects/{projectId:guid}/labels")]
    public async Task<ActionResult<LabelResponse>> CreateProjectLabel(Guid projectId, CreateProjectLabelRequest request, CancellationToken cancellationToken)
    {
        // Labels are board-scoped in the current model; BoardId is required until project-level labels are introduced.
        return ToActionResult(await service.CreateAsync(new LabelCreateRequest(request.BoardId, request.Name, request.ColorHex), cancellationToken));
    }

    [HttpPost("{labelId:guid}/task-items/{taskItemId:guid}")]
    public async Task<IActionResult> Assign(Guid labelId, Guid taskItemId, CancellationToken cancellationToken)
        => ToNoContentResult(await service.AssignAsync(taskItemId, labelId, cancellationToken));

    [HttpPost("/api/taskitems/{taskItemId:guid}/labels/{labelId:guid}")]
    public async Task<IActionResult> AssignToTask(Guid taskItemId, Guid labelId, CancellationToken cancellationToken) =>
        ToNoContentResult(await service.AssignAsync(taskItemId, labelId, cancellationToken));

    [HttpDelete("/api/taskitems/{taskItemId:guid}/labels/{labelId:guid}")]
    public async Task<IActionResult> UnassignFromTask(Guid taskItemId, Guid labelId, CancellationToken cancellationToken) =>
        ToNoContentResult(await service.UnassignAsync(taskItemId, labelId, cancellationToken));
}

public sealed record CreateProjectLabelRequest(Guid BoardId, string Name, string ColorHex);
