using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;
using TaskManager.Application.Workspaces.Dtos;
using TaskManager.Application.Workspaces.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class WorkspacesController(IWorkspaceService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<WorkspaceResponse>>> GetMine([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        ToActionResult(await service.GetMineAsync(page, pageSize, cancellationToken));

    [HttpGet("{workspaceId:guid}")]
    public async Task<ActionResult<WorkspaceResponse>> Get(Guid workspaceId, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetAsync(workspaceId, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<WorkspaceResponse>> Create(WorkspaceCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{workspaceId:guid}")]
    public async Task<ActionResult<WorkspaceResponse>> Update(Guid workspaceId, WorkspaceUpdateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.UpdateAsync(workspaceId, request, cancellationToken));

    [HttpDelete("{workspaceId:guid}")]
    public async Task<IActionResult> Delete(Guid workspaceId, CancellationToken cancellationToken) =>
        ToNoContentResult(await service.DeleteAsync(workspaceId, cancellationToken));

    [HttpPost("{workspaceId:guid}/members")]
    public async Task<ActionResult<WorkspaceMemberResponse>> AddMember(Guid workspaceId, AddWorkspaceMemberRequest request, CancellationToken cancellationToken)
        => ToActionResult(await service.AddMemberAsync(workspaceId, request, cancellationToken));
}
