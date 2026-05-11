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

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(ProjectCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));
}
