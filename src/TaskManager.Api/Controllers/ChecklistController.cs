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

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ChecklistItemResponse>> Update(Guid id, ChecklistItemUpdateRequest request, CancellationToken cancellationToken)
        => ToActionResult(await service.UpdateAsync(id, request, cancellationToken));
}
