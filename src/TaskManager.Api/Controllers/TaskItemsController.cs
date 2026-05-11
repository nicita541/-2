using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;
using TaskManager.Application.TaskItems.Dtos;
using TaskManager.Application.TaskItems.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class TaskItemsController(ITaskItemService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskItemResponse>>> Get([FromQuery] Guid boardColumnId, [FromQuery] Guid? parentTaskItemId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        ToActionResult(await service.GetByColumnAsync(boardColumnId, parentTaskItemId, page, pageSize, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> GetById(Guid id, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<TaskItemResponse>> Create(TaskItemCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> Update(Guid id, TaskItemUpdateRequest request, CancellationToken cancellationToken)
        => ToActionResult(await service.UpdateAsync(id, request, cancellationToken));

    [HttpPatch("{id:guid}/move")]
    public async Task<ActionResult<TaskItemResponse>> Move(Guid id, TaskItemMoveRequest request, CancellationToken cancellationToken)
        => ToActionResult(await service.MoveAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => ToNoContentResult(await service.SoftDeleteAsync(id, cancellationToken));
}
