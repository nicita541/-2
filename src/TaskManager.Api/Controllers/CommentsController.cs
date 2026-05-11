using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Comments.Dtos;
using TaskManager.Application.Comments.Services;
using TaskManager.Application.Common;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class CommentsController(ICommentService service) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<CommentResponse>>> Get([FromQuery] Guid taskItemId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) =>
        ToActionResult(await service.GetByTaskItemAsync(taskItemId, page, pageSize, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<CommentResponse>> Create(CommentCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));
}
