using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Boards.Dtos;
using TaskManager.Application.Boards.Services;
using TaskManager.Application.Common;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class BoardsController(IBoardService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<BoardResponse>>> Get([FromQuery] Guid projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        ToActionResult(await service.GetByProjectAsync(projectId, page, pageSize, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<BoardResponse>> Create(BoardCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));
}
