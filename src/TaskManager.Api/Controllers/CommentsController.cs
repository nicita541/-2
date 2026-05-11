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

    [HttpPost("/api/taskitems/{taskItemId:guid}/comments")]
    public async Task<ActionResult<CommentResponse>> CreateForTask(Guid taskItemId, CreateCommentBody request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(new CommentCreateRequest(taskItemId, request.Body), cancellationToken));

    [HttpPut("{commentId:guid}")]
    public async Task<ActionResult<CommentResponse>> Update(Guid commentId, CreateCommentBody request, CancellationToken cancellationToken) =>
        ToActionResult(await service.UpdateAsync(commentId, request.Body, cancellationToken));

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> Delete(Guid commentId, CancellationToken cancellationToken) =>
        ToNoContentResult(await service.DeleteAsync(commentId, cancellationToken));
}

public sealed record CreateCommentBody(string Body);
