using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Attachments.Dtos;
using TaskManager.Application.Attachments.Services;

namespace TaskManager.Api.Controllers;

[Authorize]
public sealed class AttachmentsController(IAttachmentService service) : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AttachmentResponse>> Create(AttachmentCreateRequest request, CancellationToken cancellationToken) =>
        ToActionResult(await service.CreateAsync(request, cancellationToken));
}
