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

    [HttpPost("{labelId:guid}/task-items/{taskItemId:guid}")]
    public async Task<IActionResult> Assign(Guid labelId, Guid taskItemId, CancellationToken cancellationToken)
        => ToNoContentResult(await service.AssignAsync(taskItemId, labelId, cancellationToken));
}
