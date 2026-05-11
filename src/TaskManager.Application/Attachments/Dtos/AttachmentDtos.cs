namespace TaskManager.Application.Attachments.Dtos;

public sealed record AttachmentCreateRequest(Guid TaskItemId, string FileName, string ContentType, string Url, long SizeBytes);
public sealed record AttachmentResponse(Guid Id, Guid TaskItemId, string FileName, string ContentType, string Url, long SizeBytes);
