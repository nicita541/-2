using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Frontend.Dtos;
using TaskManager.Application.Frontend.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class FrontendReadController(IFrontendReadService service) : ApiControllerBase
{
    [HttpGet("/api/projects/{projectId:guid}/overview")]
    public async Task<ActionResult<ProjectOverviewResponse>> ProjectOverview(Guid projectId, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetProjectOverviewAsync(projectId, cancellationToken));

    [HttpGet("/api/boards/{boardId:guid}/kanban")]
    public async Task<ActionResult<KanbanBoardResponse>> BoardKanban(Guid boardId, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetBoardKanbanAsync(boardId, cancellationToken));

    [HttpGet("/api/taskitems/{taskItemId:guid}/details")]
    public async Task<ActionResult<TaskDetailsResponse>> TaskDetails(Guid taskItemId, CancellationToken cancellationToken) =>
        ToActionResult(await service.GetTaskDetailsAsync(taskItemId, cancellationToken));
}
