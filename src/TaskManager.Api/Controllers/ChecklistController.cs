using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Checklist.Dtos;
using TaskManager.Application.Checklist.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class ChecklistController(IChecklistService service) : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ChecklistItemResponse>> Create(ChecklistItemCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));

    [HttpPost("/api/taskitems/{taskItemId:guid}/checklist")]
    public async Task<ActionResult<ChecklistItemResponse>> CreateForTask(Guid taskItemId, CreateChecklistBody request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(new ChecklistItemCreateRequest(taskItemId, request.Text, request.Position), cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ChecklistItemResponse>> Update(Guid id, ChecklistItemUpdateRequest request, CancellationToken cancellationToken)
        => ToActionResult(await service.UpdateAsync(id, request, cancellationToken));

    [HttpPost("{id:guid}/toggle")]
    public async Task<ActionResult<ChecklistItemResponse>> Toggle(Guid id, CancellationToken cancellationToken) =>
        ToActionResult(await service.ToggleAsync(id, cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) =>
        ToNoContentResult(await service.DeleteAsync(id, cancellationToken));
}

public sealed record CreateChecklistBody(string Text, int Position);
