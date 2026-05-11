using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Columns.Dtos;
using TaskManager.Application.Columns.Services;
using TaskManager.Application.Common;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class ColumnsController(IColumnService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ColumnResponse>>> Get([FromQuery] Guid boardId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        ToActionResult(await service.GetByBoardAsync(boardId, page, pageSize, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ColumnResponse>> Create(ColumnCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));
}
