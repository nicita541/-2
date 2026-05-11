using TaskManager.Application.Attachments.Dtos;
using TaskManager.Application.Common;

namespace TaskManager.Application.Attachments.Services;

public interface IAttachmentService
{
    Task<Result<AttachmentResponse>> CreateAsync(AttachmentCreateRequest request, CancellationToken cancellationToken);
}
