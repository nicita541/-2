using TaskManager.Application.Abstractions;
using TaskManager.Application.Attachments.Dtos;
using TaskManager.Application.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Attachments.Services;

public sealed class AttachmentService(IApplicationDbContext db, ICurrentUserService currentUser) : IAttachmentService
{
    public async Task<Result<AttachmentResponse>> CreateAsync(AttachmentCreateRequest request, CancellationToken cancellationToken)
    {
        var attachment = new Attachment
        {
            TaskItemId = request.TaskItemId,
            UploadedById = currentUser.UserId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            Url = request.Url,
            SizeBytes = request.SizeBytes
        };
        db.Add(attachment);
        await db.SaveChangesAsync(cancellationToken);
        return Result<AttachmentResponse>.Success(new AttachmentResponse(attachment.Id, attachment.TaskItemId, attachment.FileName, attachment.ContentType, attachment.Url, attachment.SizeBytes));
    }
}
