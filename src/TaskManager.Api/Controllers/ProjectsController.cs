using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;
using TaskManager.Application.Projects.Dtos;
using TaskManager.Application.Projects.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class ProjectsController(IProjectService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProjectResponse>>> Get([FromQuery] Guid workspaceId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        ToActionResult(await service.GetByWorkspaceAsync(workspaceId, page, pageSize, cancellationToken));

    [HttpGet("{projectId:guid}")]
    public async Task<ActionResult<ProjectResponse>> Get(Guid projectId, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetAsync(projectId, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(ProjectCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{projectId:guid}")]
    public async Task<ActionResult<ProjectResponse>> Update(Guid projectId, ProjectUpdateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.UpdateAsync(projectId, request, cancellationToken));

    [HttpDelete("{projectId:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, CancellationToken cancellationToken) =>
        ToNoContentResult(await service.DeleteAsync(projectId, cancellationToken));
}
